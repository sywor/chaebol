using System;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Fabricator
{
    [Serializable]
    [GraphContextMenuItem("Fabricator", "Natural resource")]
    public class NaturalResourceSource : AbstractFabricatorNode
    {
        [SerializeField] private Resource resource = new Resource();

        private Rect tmpRect;
        
        public NaturalResourceSource(int _id, Graph _parent) : base(_id, _parent)
        {
            var resourceOutputSocket = new OutputSocket(this, typeof(IResourceConnection));
            Sockets.Add(resourceOutputSocket);
            
            Width = 140;
            Height = 175;
        }

        protected override void OnGUI()
        {
            var guiStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal = {textColor = UnityEngine.Color.white}
            };

            SetResourceName(guiStyle);
            SetExtractionTime(guiStyle);
            SetVolume(guiStyle);
            SetWeigth(guiStyle);
        }

        private void SetResourceName(GUIStyle _guiStyle)
        {
            tmpRect.Set(10, 10, 120, 10);
            GUI.Label(tmpRect, new GUIContent("Resource name:"), _guiStyle);

            tmpRect.Set(10, 25, 120, 15);
            resource.Name = GUI.TextField(tmpRect, resource.Name);
        }
        
        private void SetExtractionTime(GUIStyle _guiStyle)
        {
            tmpRect.Set(10, 45, 120, 10);
            GUI.Label(tmpRect, new GUIContent("Extraction time:"), _guiStyle);
            
            var tmpTime = Math.Abs(resource.WorkTime.TotalSeconds) < 0.1 ? "" : resource.WorkTime.TotalSeconds.ToString();

            tmpRect.Set(10, 60, 120, 15);
            tmpTime = GUI.TextField(tmpRect, tmpTime);
            
            try
            {
                resource.WorkTime = TimeSpan.FromSeconds(Convert.ToDouble(tmpTime));
            }
            catch (OverflowException e){}
            catch (FormatException e) {}
            catch (ArgumentException e){}
        }
        
        private void SetVolume(GUIStyle _guiStyle)
        {
            tmpRect.Set(10, 80, 120, 10);
            GUI.Label(tmpRect, new GUIContent("Volume:"), _guiStyle);

            var tmpVolume = Math.Abs(resource.Volume) < 0.001 ? "" : resource.Volume.ToString("N2");
            
            tmpRect.Set(10, 95, 120, 15);
            tmpVolume = GUI.TextField(tmpRect, tmpVolume);
            
            try
            {
                resource.Volume = Convert.ToSingle(tmpVolume);
            }
            catch (FormatException e)
            {
                resource.Volume = 0.0f;
            }
        }
        
        private void SetWeigth(GUIStyle _guiStyle)
        {
            tmpRect.Set(10, 115, 120, 10);
            GUI.Label(tmpRect, new GUIContent("Weigth:"), _guiStyle);

            var tmpWeigth = Math.Abs(resource.Weigth) < 0.001 ? "" : resource.Weigth.ToString("N2");
            
            tmpRect.Set(10, 130, 120, 15);
            tmpWeigth = GUI.TextField(tmpRect, tmpWeigth);
            
            try
            {
                resource.Weigth = Convert.ToSingle(tmpWeigth);
            }
            catch (FormatException e)
            {
                resource.Weigth = 0.0f;
            }
        }

        public override void Update()
        {

        }

        public override Resource GetResource(OutputSocket _outSocket, Request _request)
        {
            return resource;
        }
    }
}
