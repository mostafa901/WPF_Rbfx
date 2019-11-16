using ImGuiNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Urho3DNet;
using Application = Urho3DNet.Application;
using Color = Urho3DNet.Color;

namespace WPF_Rbfx
{
	/// <summary>
	/// Interaction logic for uc_rbfx.xaml
	/// </summary>
	public partial class uc_rbfx : UserControl
	{
		public uc_rbfx()
		{
			InitializeComponent();
			try
			{
				//External exe inside WPF Window 
				System.Windows.Forms.Panel _pnlSched = new System.Windows.Forms.Panel();
				WindowsFormsHost windowsFormsHost1 = new WindowsFormsHost();
				windowsFormsHost1.Child = _pnlSched;
				_Grid.Children.Add(windowsFormsHost1);

				DemoApplication.Parent = _pnlSched.Handle;
				Loaded += delegate
				{

					Task.Run(() =>
					{
						using (var context = new Context())
						{
							using (var application = new DemoApplication(context))
							{
								application.Run();
							}
						}
					});

					
				};

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}


	}

	class DemoApplication : Application
	{
		public static IntPtr Parent;
		private Scene _scene;
		private Viewport _viewport;
		private Node _camera;
		private Node _cube;
		private Node _light;

		public DemoApplication(Context context) : base(context)
		{
		}

		protected override void Dispose(bool disposing)
		{
			Engine.Renderer.SetViewport(0, null);    // Enable disposal of viewport by making it unreferenced by engine.
			_viewport.Dispose();
			_scene.Dispose();
			_camera.Dispose();
			_cube.Dispose();
			_light.Dispose();
			base.Dispose();
		}

		public override void Setup()
		{
			engineParameters_[Urho3D.EpFullScreen] = false;
			engineParameters_[Urho3D.EpExternalWindow] = Parent; //todo: there is a know dispose bug when closing WPF window
			engineParameters_[Urho3D.EpWindowWidth] = 800;
			engineParameters_[Urho3D.EpWindowHeight] = 600;
			engineParameters_[Urho3D.EpWindowTitle] = "Hello C#";
			string ModelFolder = @"D:\Revit_API\Downloaded_Library\Source\rbfx\cmake-build\bin\Data";
			engineParameters_[Urho3D.EpResourcePrefixPaths] = $"{ModelFolder};{ModelFolder}/..;";
		}

		public override void Start()
		{
			UI.Input.SetMouseVisible(true);

			// Viewport
			_scene = new Scene(Context);
			_scene.CreateComponent<Octree>();

			_camera = _scene.CreateChild("Camera");
			_viewport = new Viewport(Context);
			_viewport.Scene = _scene;
			_viewport.Camera = (_camera.CreateComponent<Camera>());
			Engine.Renderer.SetViewport(0, _viewport);
			// Background
			Engine.Renderer.DefaultZone.FogColor = (new Color(0.5f, 0.5f, 0.7f));

			// Scene
			_camera.Position = (new Vector3(0, 2, -2));
			_camera.LookAt(Vector3.Zero);

			// Cube
			_cube = _scene.CreateChild("Cube");
			var model = _cube.CreateComponent<StaticModel>();
			model.SetModel(Cache.GetResource<Urho3DNet.Model>("Models/Box.mdl"));
			model.SetMaterial(0, Cache.GetResource<Material>("Materials/Stone.xml"));
			var rotator = _cube.CreateComponent<RotateObject>();

			// Light
			_light = _scene.CreateChild("Light");
			_light.CreateComponent<Light>();
			_light.Position = (new Vector3(0, 2, -1));
			_light.LookAt(Vector3.Zero);

			SubscribeToEvent(E.Update, args =>
			{ 
				SetupMenu(); 
			});
		}

		bool openup = false;
		private void SetupMenu()
		{
			if (ImGui.BeginMainMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("Show Message", ""))
					{
						showmessage();
					}

					if (ImGui.MenuItem("PopupModaltest", ""))
					{
						//import();
						openup = true;
					}

					if (ImGui.MenuItem("Popuptest", ""))
					{
						ImGui.OpenPopup("pop");
					}

					ImGui.EndMenu();
				}

				ImGui.EndMainMenuBar();
			}

			if (openup) import();

			if (ImGui.BeginPopup("pop"))
			{
				if (ImGui.Button("Test"))
				{
					showmessage("test from Popup not modal");
				}
			}
		}

		void showmessage(string message = "Hello")
		{
			new Urho3DNet.MessageBox(Context, message, "first Click");
		}

		void import()
		{
			//ImGui.ShowDemoWindow(ref openup);

			if (ImGui.BeginPopupModal("p", ref openup))
			{
				ImGui.Text("something?.");
				if (ImGui.Button("OK"))
				{
					showmessage("test from ModalPopup");
					ImGui.CloseCurrentPopup();
				}
				if (ImGui.Button("Cancel")) ImGui.CloseCurrentPopup();

				ImGui.EndPopup();


			}
		}





	}






	[ObjectFactory]
	class RotateObject : LogicComponent
	{
		public RotateObject(Context context) : base(context)
		{
			UpdateEventMask = UpdateEvent.UseUpdate;

		}

		public override void Update(float timeStep)
		{
			var d = new Quaternion(10 * timeStep, 20 * timeStep, 30 * timeStep);
			Node.Rotate(d);
		}
	}
}
