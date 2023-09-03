import { Room, Client } from "colyseus";
import { Schema, type, MapSchema } from "@colyseus/schema";

export class Player extends Schema {
    @type("number") x = Math.floor(Math.random() * 256)-128;
    @type("number") z = Math.floor(Math.random() * 256)-128;
    @type("uint8") d = 1;
    @type("uint8") skin = 0;
}
// Состояние комнаты(то в каком состоянии находятся поля), на основе данных(полей) создаются классы на стороне клиента
// Изменения данных, между сервером и клиентом реализованиы системой событий (клиент подписывается на события)
export class State extends Schema {
    // создается экземпляр игрока и записывается в список игроков под ключом ID нашей сессии
    @type({ map: Player }) players = new MapSchema<Player>();

    createPlayer(sessionId: string, data: any, skin: number) {
        const player = new Player()

        player.skin = skin;

        this.players.set(sessionId, player);
    }

    removePlayer(sessionId: string) {
        this.players.delete(sessionId);
    }

    movePlayer (sessionId: string, movement: any) {
        this.players.get(sessionId).x = movement.x
        this.players.get(sessionId).z = movement.z
    }

    changeSkin (sessionId: string, data: any) {
        this.players.get(sessionId).skin = data
        console.log(data); // проверка в консоли идет ли сообщение
    }
}
// экземпляр комнаты Room, с данными от класса State со стороны клиента
export class StateHandlerRoom extends Room<State> {
    maxClients = 40;
    skins: number[] = [0]; // создаем массив чисел

    mixArray(arr){
        var currentIndex = arr.length;
        var tmpValue, randomIndex;

        while(currentIndex !== 0){
            randomIndex = Math.floor(Math.random() * currentIndex);
            currentIndex -= 1;
            tmpValue = arr[currentIndex];
            arr[currentIndex] = arr[randomIndex];
            arr[randomIndex] = tmpValue;
        }
    }

    onCreate (options) {
        for(var i = 1; i < options.skins; i++){ // заполняем массив количеством элементов которое у нас есть
            this.skins.push(i);
        }

        this.mixArray(this.skins); // перемешиваем массиы

        this.setState(new State()); // создание экземпляра класса State

        this.onMessage("move", (client, data) => {
            this.state.movePlayer(client.sessionId, data);
        });

        this.onMessage("skin", (client, data) => {
            this.state.changeSkin(client.sessionId, data);
        });
    }

    onAuth(client, options, req) {
        return true;
    }

    onJoin (client: Client, data: any) {
        //if(this.clients.length > 1) this.lock();

        const skin = this.skins[this.clients.length - 1];
        this.state.createPlayer(client.sessionId, data, skin);
    }

    onLeave (client) {
        this.state.removePlayer(client.sessionId);
    }

    onDispose () {
        console.log("Dispose StateHandlerRoom");
    }
}
