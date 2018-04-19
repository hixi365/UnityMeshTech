using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mesh描画関係のコンポーネントを追加
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

// 物理演算用のコンポーネントを追加
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]

public class MeshCylinderGenerator : MonoBehaviour {

	// スケール
	[SerializeField]
	private float radius = 1.0f;
	[SerializeField]
	private float height = 1.0f;

	// 裏表
	[SerializeField]
	private bool enableInside = true;
	[SerializeField]
	private bool enableOutside = true;

	// 円周分割数
	[SerializeField]
	private int numDiv = 30;

	private Mesh mesh;

	void Awake()
	{

		// RigiedBody
		Rigidbody rb = GetComponent<Rigidbody>();
		if (null != rb)
		{
			rb.isKinematic = true;
			rb.useGravity = false;
		}

	}

	void Start()
	{

		// 裏表なしなら処理をしない
		if(enableInside == false && enableOutside == false)
		{

			return;

		}

		mesh = new Mesh();

		List<Vector3> vertex = new List<Vector3>();
		List<Vector3> normal = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> id = new List<int>();

		for (int i = 0; i < numDiv; i++)
		{

			// 円周方向 (角度計算用)
			int i_0 = (i + 0) % numDiv * (360 / numDiv);
			int i_1 = (i + 1) % numDiv * (360 / numDiv);

			// 円周方向 (角度)
			float a_0 = i_0 / 180.0f * (Mathf.PI);
			float a_1 = i_1 / 180.0f * (Mathf.PI);

			// 円周方向 (三角関数算出)
			float r_0_c = Mathf.Cos(a_0);
			float r_1_c = Mathf.Cos(a_1);
			float r_0_s = Mathf.Sin(a_0);
			float r_1_s = Mathf.Sin(a_1);

			// 高さ方向
			float h_0 = +height;
			float h_1 = -height;

			// 座標計算
			Vector3 pos0 = new Vector3(r_0_c * radius, h_0, r_0_s * radius);
			Vector3 pos1 = new Vector3(r_1_c * radius, h_0, r_1_s * radius);
			Vector3 pos2 = new Vector3(r_0_c * radius, h_1, r_0_s * radius);
			Vector3 pos3 = new Vector3(r_1_c * radius, h_1, r_1_s * radius);

			// 法線計算 (外向き)
			Vector3 n0 = Vector3.Normalize(Vector3.Cross(pos1 - pos0, pos2 - pos0));
			Vector3 n1 = Vector3.Normalize(Vector3.Cross(pos3 - pos1, pos0 - pos1));
			Vector3 n2 = Vector3.Normalize(Vector3.Cross(pos0 - pos2, pos3 - pos2));
			Vector3 n3 = Vector3.Normalize(Vector3.Cross(pos2 - pos3, pos1 - pos3));

			// 法線計算 (内向き)
			Vector3 n4 = -n0;
			Vector3 n5 = -n1;
			Vector3 n6 = -n2;
			Vector3 n7 = -n3;

			// 表面
			if (enableOutside == true)
			{

				// インデックス計算用に取り出す
				int length = vertex.Count;

				// 頂点の追加
				vertex.Add(pos0);
				vertex.Add(pos1);
				vertex.Add(pos2);
				vertex.Add(pos3);

				// 法線の追加
				normal.Add(n0);
				normal.Add(n1);
				normal.Add(n2);
				normal.Add(n3);

				// インデックスの追加
				id.Add(length + 0);
				id.Add(length + 1);
				id.Add(length + 2);
				id.Add(length + 2);
				id.Add(length + 1);
				id.Add(length + 3);

			}

			// 裏面
			if (enableInside == true)
			{

				// インデックス計算用に取り出す
				int length = vertex.Count;

				// 頂点の追加
				vertex.Add(pos0);
				vertex.Add(pos1);
				vertex.Add(pos2);
				vertex.Add(pos3);

				// 法線の追加
				normal.Add(n4);
				normal.Add(n5);
				normal.Add(n6);
				normal.Add(n7);

				// インデックスの追加
				id.Add(length + 2);
				id.Add(length + 1);
				id.Add(length + 0);
				id.Add(length + 3);
				id.Add(length + 1);
				id.Add(length + 2);

			}

		}

		// meshへ追加
		mesh.vertices = vertex.ToArray();
		mesh.normals = normal.ToArray();
		//	mesh.uv = uv.ToArray();
		mesh.triangles = id.ToArray();

		// meshFilterへ追加
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.sharedMesh = mesh;

		// 接触判定へ追加
		MeshCollider collider = (MeshCollider)GetComponent<MeshCollider>();
		if (collider)
		{
			collider.sharedMesh = mesh;
		}

	}
}
