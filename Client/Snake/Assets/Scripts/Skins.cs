using UnityEngine;

public class Skins : MonoBehaviour
{
    [SerializeField] public Material[] Materials;

    public int Length { get { return Materials.Length; } }

    public Material GetMaterial(byte index) // ���������� �������� �� �������
    {
        if (Materials.Length <= index) return Materials[0];

        return Materials[index];
    }
}
