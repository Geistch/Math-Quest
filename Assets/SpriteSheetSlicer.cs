using UnityEngine;
using UnityEditor;

public class SpriteSheetSlicer : MonoBehaviour
{
    [MenuItem("Ferramentas/Fatiar Sprite Sheet")]
    static void Fatiar()
    {
        // Caminho do sprite selecionado
        Texture2D texture = Selection.activeObject as Texture2D;

        if (texture == null)
        {
            Debug.LogError("Selecione uma imagem no formato sprite sheet na aba Project.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Multiple;

        // Defina aqui o tamanho de cada frame (ajuste conforme seu sprite sheet)
        int frameWidth = 64;
        int frameHeight = 64;

        int columns = texture.width / frameWidth;
        int rows = texture.height / frameHeight;

        SpriteMetaData[] newData = new SpriteMetaData[columns * rows];
        int index = 0;

        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < columns; x++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = "frame_" + index;
                smd.rect = new Rect(x * frameWidth, y * frameHeight, frameWidth, frameHeight);
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = 9; // custom pivot
                newData[index++] = smd;
            }
        }

        ti.spritesheet = newData;
        EditorUtility.SetDirty(ti);
        ti.SaveAndReimport();

        Debug.Log("Sprite sheet fatiado com sucesso!");
    }
}
