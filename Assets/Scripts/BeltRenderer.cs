using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BeltRenderer : MonoBehaviour
{

	public int Count;
	public float HasItemChance = 1.0f;

	public Mesh Mesh;
	public Material Material;
	public Material MaterialItem1;
	public Material MaterialItem2;

	public ShadowCastingMode ShadowCasting = ShadowCastingMode.Off;
	public bool ReceiveShadows = true;

	private MaterialPropertyBlock MPB;
	private MaterialPropertyBlock MPBItem1;
	private MaterialPropertyBlock MPBItem2;

	private Bounds bounds;
	private ComputeBuffer instancesBuffer;
	private ComputeBuffer argsBuffer;
	private ComputeBuffer instancesBufferItem1;
	private ComputeBuffer argsBufferItem1;
	private ComputeBuffer instancesBufferItem2;
	private ComputeBuffer argsBufferItem2;
	private List<ItemInstanceData> item1 = new List<ItemInstanceData>();
	private List<ItemInstanceData> item2 = new List<ItemInstanceData>();

	private struct InstanceData
	{
		public Matrix4x4 Matrix;
		public Matrix4x4 MatrixInverse;

		public static int Size()
		{
			return sizeof(float) * 4 * 4
				+ sizeof(float) * 4 * 4;
		}
	}

	private struct ItemInstanceData
	{
		public Matrix4x4 Matrix;
		public Matrix4x4 MatrixInverse;
		public Color Color;

		public static int Size()
		{
			return sizeof(float) * 4 * 4
				+ sizeof(float) * 4 * 4
				+ sizeof(float) * 4;
		}
	}

	public void OnEnable()
	{
		MPB = new MaterialPropertyBlock();
		MPBItem1 = new MaterialPropertyBlock();
		MPBItem2 = new MaterialPropertyBlock();
		bounds = new Bounds(Vector3.zero, new Vector3(100000, 100000, 100000));
		InitializeBuffers();
	}

	private ComputeBuffer GetArgsBuffer(uint count)
	{
		uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
		args[0] = (uint)Mesh.GetIndexCount(0);
		args[1] = (uint)count;
		args[2] = (uint)Mesh.GetIndexStart(0);
		args[3] = (uint)Mesh.GetBaseVertex(0);
		args[4] = 0;

		ComputeBuffer buffer = new ComputeBuffer(args.Length, sizeof(uint), ComputeBufferType.IndirectArguments);
		buffer.SetData(args);
		return buffer;
	}

	private void InitializeBuffers()
	{
		int amountPerLine = (int)Mathf.Sqrt(Count);
		int lines = amountPerLine;

		InstanceData[] instances = new InstanceData[Count];
		item1.Clear();
		item2.Clear();

		for (int x = 0; x < lines; x++)
		{
			bool hasItem = Random.Range(0.0f, 1.0f) <= HasItemChance;
			List<ItemInstanceData> itemSelect = Random.Range(0.0f, 1.0f) <= 0.5f ? item1 : item2;
			Color itemColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f) * Random.Range(1.0f, 8.0f);

			for (int z = 0; z < amountPerLine; z++)
			{
				InstanceData data = new InstanceData();

				Vector3 position = new Vector3(x, 0, z);
				Quaternion rotation = Quaternion.Euler(0, 90, 0);
				Vector3 scale = new Vector3(1, 0.01f, 1);

				data.Matrix = Matrix4x4.TRS(position, rotation, scale);
				data.MatrixInverse = data.Matrix.inverse;
				instances[z * amountPerLine + x] = data;

				if (hasItem)
				{
					ItemInstanceData dataItem = new ItemInstanceData();

					Vector3 positionItem = new Vector3(x, 0.3f, z);
					Quaternion rotationItem = Quaternion.Euler(0, 0, 0);
					Vector3 scaleItem = new Vector3(0.5f, 0.5f, 0.5f);

					dataItem.Matrix = Matrix4x4.TRS(positionItem, rotationItem, scaleItem);
					dataItem.MatrixInverse = data.Matrix.inverse;
					dataItem.Color = itemColor;
					itemSelect.Add(dataItem);
				}
			}
		}

		argsBuffer = GetArgsBuffer((uint)instances.Length);
		instancesBuffer = new ComputeBuffer(instances.Length, InstanceData.Size());
		instancesBuffer.SetData(instances);
		Material.SetBuffer("_PerInstanceData", instancesBuffer);

		if (item1.Count > 0)
		{
			argsBufferItem1 = GetArgsBuffer((uint)item1.Count);
			instancesBufferItem1 = new ComputeBuffer(item1.Count, ItemInstanceData.Size());
			instancesBufferItem1.SetData(item1);
			MaterialItem1.SetBuffer("_PerInstanceItemData", instancesBufferItem1);
		}

		if (item2.Count > 0)
		{
			argsBufferItem2 = GetArgsBuffer((uint)item2.Count);
			instancesBufferItem2 = new ComputeBuffer(item2.Count, ItemInstanceData.Size());
			instancesBufferItem2.SetData(item2);
			MaterialItem2.SetBuffer("_PerInstanceItemData", instancesBufferItem2);
		}
	}

	public void Update()
	{
		Graphics.DrawMeshInstancedIndirect(Mesh, 0, Material, bounds, argsBuffer, 0, MPB, ShadowCasting, ReceiveShadows);

		if (HasItemChance > 0.0f)
		{
			if (item1.Count > 0)
			{
				Graphics.DrawMeshInstancedIndirect(Mesh, 0, MaterialItem1, bounds, argsBufferItem1, 0, MPBItem1, ShadowCasting, ReceiveShadows);
			}

			if (item2.Count > 0)
			{
				Graphics.DrawMeshInstancedIndirect(Mesh, 0, MaterialItem2, bounds, argsBufferItem2, 0, MPBItem2, ShadowCasting, ReceiveShadows);
			}
		}
	}

	private void OnDisable()
	{
		if (instancesBuffer != null)
		{
			instancesBuffer.Release();
			instancesBuffer = null;
		}
		
		if (instancesBufferItem1 != null)
		{
			instancesBufferItem1.Release();
			instancesBufferItem1 = null;
		}
		
		if (instancesBufferItem2 != null)
		{
			instancesBufferItem2.Release();
			instancesBufferItem2 = null;
		}
		
		if (argsBuffer != null)
		{
			argsBuffer.Release();
			argsBuffer = null;
		}
		
		if (argsBufferItem1 != null)
		{
			argsBufferItem1.Release();
			argsBufferItem1 = null;
		}

		if (argsBufferItem2 != null)
		{
			argsBufferItem2.Release();
			argsBufferItem2 = null;
		}
	}
}