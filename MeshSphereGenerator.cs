using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mesh描画関係のコンポーネントを追加
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

// 物理演算用のコンポーネントを追加
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]

public class MeshSphereGenerator : MonoBehaviour {

	// スケール
	[SerializeField]
	private float radius = 1.0f;

	// 裏表
	[SerializeField]
	private bool enableInside = true;
	[SerializeField]
	private bool enableOutside = true;

	// 円周分割数
	[SerializeField]
	private int numDivXZ = 30;  // xz平面
	[SerializeField]
	private int numDivY = 15;   // y平面

	// y分割生成範囲 -1 ~ 1
	[SerializeField]
	private float clipMinY = -1;  // -1
	[SerializeField]
	private float clipMaxY = 1;   // 1


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
		if (enableInside == false && enableOutside == false)
		{

			return;

		}

		// y分割生成範囲がおかしいなら処理をしない
		if(clipMinY >= clipMaxY || clipMinY < -1 || clipMaxY > 1)
		{

			return;

		}

		mesh = new Mesh();

		List<Vector3> vertex = new List<Vector3>();
		List<Vector3> normal = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> id = new List<int>();

		for (int j = 0; j < numDivY; j++)
		{
			for (int i = 0; i < numDivXZ; i++)
			{

				// XZ方向 (角度計算用) : i
				int i_0 = (i + 0) % numDivXZ * (360 / numDivXZ);
				int i_1 = (i + 1) % numDivXZ * (360 / numDivXZ);

				// 円周方向 (角度)
				float ia_0 = i_0 / 180.0f * (Mathf.PI);
				float ia_1 = i_1 / 180.0f * (Mathf.PI);

				// Y方向前処理
				float minY = 90 * clipMinY;
				float maxY = 90 * clipMaxY;
				float diffY = maxY - minY;

				// Y方向 (角度計算用) : j
				int j_0 = (j + 0) * ((int)diffY / numDivY);
				int j_1 = (j + 1) * ((int)diffY / numDivY);

				// 円周方向 (角度)
				float ja_0 = (j_0 + minY) / 180.0f * (Mathf.PI);
				float ja_1 = (j_1 + minY) / 180.0f * (Mathf.PI);

				// 座標計算
				Vector3 pos0 = new Vector3(Mathf.Cos(ja_1) * Mathf.Cos(ia_0), Mathf.Sin(ja_1), Mathf.Cos(ja_1) * Mathf.Sin(ia_0)) * radius;
				Vector3 pos1 = new Vector3(Mathf.Cos(ja_1) * Mathf.Cos(ia_1), Mathf.Sin(ja_1), Mathf.Cos(ja_1) * Mathf.Sin(ia_1)) * radius;
				Vector3 pos2 = new Vector3(Mathf.Cos(ja_0) * Mathf.Cos(ia_0), Mathf.Sin(ja_0), Mathf.Cos(ja_0) * Mathf.Sin(ia_0)) * radius;
				Vector3 pos3 = new Vector3(Mathf.Cos(ja_0) * Mathf.Cos(ia_1), Mathf.Sin(ja_0), Mathf.Cos(ja_0) * Mathf.Sin(ia_1)) * radius;

				// 法線計算 (外向き)
				Vector3 n0 = Vector3.Normalize(pos0 - Vector3.zero);
				Vector3 n1 = Vector3.Normalize(pos1 - Vector3.zero);
				Vector3 n2 = Vector3.Normalize(pos2 - Vector3.zero);
				Vector3 n3 = Vector3.Normalize(pos3 - Vector3.zero);

				// 法線計算 (内向き)
				Vector3 n4 = -n0;
				Vector3 n5 = -n1;
				Vector3 n6 = -n2;
				Vector3 n7 = -n3;

				//Vector2 tex0 = new Vector2(Mathf.Cos(ia_1) * Mathf.Cos(ja_0) * texrad / 2 + 0.5f, Mathf.Cos(ia_1) * Mathf.Sin(ja_0) * texrad / 2 + 0.5f);
				//Vector2 tex1 = new Vector2(Mathf.Cos(ia_1) * Mathf.Cos(ja_1) * texrad / 2 + 0.5f, Mathf.Cos(ia_1) * Mathf.Sin(ja_1) * texrad / 2 + 0.5f);
				//Vector2 tex2 = new Vector2(Mathf.Cos(ia_0) * Mathf.Cos(ja_0) * texrad / 2 + 0.5f, Mathf.Cos(ia_0) * Mathf.Sin(ja_0) * texrad / 2 + 0.5f);
				//Vector2 tex3 = new Vector2(Mathf.Cos(ia_0) * Mathf.Cos(ja_1) * texrad / 2 + 0.5f, Mathf.Cos(ia_0) * Mathf.Sin(ja_1) * texrad / 2 + 0.5f);

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
