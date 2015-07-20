using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

/// <summary>
/// テクスチャを分割する
/// </summary>
public class SplitTexture : EditorWindow {

	public static Texture2D srcTexture;
	public static int split_width;
	public static int split_height;

	[MenuItem("Window/split texture")]
	public static void OpenWindow () {
		EditorWindow.GetWindow(typeof(SplitTexture), false, "SplitTexture");
	}

	void OnGUI() {
		srcTexture = EditorGUILayout.ObjectField("Texture", srcTexture, typeof(Texture2D), false) as Texture2D;

		split_width = EditorGUILayout.IntField("width", split_width);
		split_height = EditorGUILayout.IntField("height", split_height);

		if (GUILayout.Button("Split", GUILayout.Width(100), GUILayout.Height(40)))
		{
			// 分割実行
			Texture2D[] split_textures = SplitTex(srcTexture, split_width, split_height, true);
		}
	}


	/// <summary>
	/// テクスチャ分割
	/// </summary>
	/// <returns>分割されたテクスチャを配列で返す</returns>
	public static Texture2D[] SplitTex(Texture2D texture, int w, int h, bool autoSave)
	{
		if (texture == null)
			throw new System.ArgumentNullException ("texture");
		if (texture == null || w <= 0 || h <= 0) { return null; }

		string srcPath = AssetDatabase.GetAssetPath(texture);
		string srcName = texture.name;
		int num_w = Mathf.FloorToInt((float)texture.width / w);
		int num_h = Mathf.FloorToInt((float)texture.height / h);
		System.Collections.Generic.List<Texture2D> texs = new System.Collections.Generic.List<Texture2D>();

		// 格納用フォルダの作成
		string parentFolder = srcPath.Remove(srcPath.LastIndexOf('/'));
		string newFolder = srcName+"_split";
		AssetDatabase.CreateFolder(parentFolder, newFolder);

		for (int iw = 0; iw < num_w; iw++) {
			for (int ih = 0; ih < num_h; ih++) {
				// ピクセルコピー
				Texture2D tmp = new Texture2D(w, h, TextureFormat.RGBA32, false);
				tmp.SetPixels( texture.GetPixels(w * iw, h * ih, w, h) );

				tmp.name = srcName + "_" + (num_h-ih-1) + "_" + iw;// ex) image_0_0.png
				texs.Add (tmp);

				// 保存処理
				if (autoSave)
				{
					byte [] pngData = tmp.EncodeToPNG();   // pngのバイト情報を取得.

					// 保存パス
					string filePath = srcPath.Remove(srcPath.LastIndexOf('/')+1) + srcName + "_split/" + tmp.name + ".png";
//					Debug.Log("path  : " + filePath);

					if (filePath.Length > 0) {
						// pngファイル保存.
						File.WriteAllBytes(filePath, pngData);
					}
				}
			}
		}

		if (autoSave) {
			AssetDatabase.Refresh();
		}

		return texs.ToArray();
	}
}
