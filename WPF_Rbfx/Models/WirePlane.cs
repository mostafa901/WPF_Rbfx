using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace WPF_Rbfx.Models
{
	public class WirePlane : Component
	{
		CustomGeometry geom;
		private int size = 50;
		private float scale = 1f;
		private Color color = new Color(1f, 0.0f, 0.7f);

		public WirePlane(Context context) : base(context)
		{

		}

		public int Size
		{
			get { return size; }
			set
			{
				size = value;
				Reload();
			}
		}

		public float Scale
		{
			get { return scale; }
			set
			{
				scale = value;
				Reload();
			}
		}

		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				Reload();
			}
		}

		protected override void OnNodeSet(Node node)
		{
			base.OnNodeSet(node);
			Reload();
		}


		void Reload()
		{
			if (geom != null && !geom.IsZoneDirty())
				geom.Remove();

			if (Node == null || Node.IsDirty())
				return;

			geom = Node.CreateComponent<CustomGeometry>();
			geom.BeginGeometry(0, PrimitiveType.LineList);

			var teq = Cache.GetResource<Technique>("Textures/NoTextureUnlit.xml");

			var material = new Urho3DNet.Material(Context);
			material.SetTechnique(0, teq, MaterialQuality.QualityLow);
			geom.SetMaterial(material);

			var halfSize = Size / 2;
			for (int i = -halfSize; i <= halfSize; i++)
			{
				//x
				geom.DefineVertex(new Vector3(i, 0, -halfSize) * Scale);
				geom.DefineColor(Color);
				geom.DefineVertex(new Vector3(i, 0, halfSize) * Scale);
				geom.DefineColor(Color);

				//z
				geom.DefineVertex(new Vector3(-halfSize, 0, i) * Scale);
				geom.DefineColor(Color);
				geom.DefineVertex(new Vector3(halfSize, 0, i) * Scale);
				geom.DefineColor(Color);
			}

			geom.Commit();
		}
	}
}
