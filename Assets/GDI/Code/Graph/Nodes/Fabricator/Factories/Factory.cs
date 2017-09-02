using System;
using System.Collections.Generic;
using System.Linq;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.GDI.Code.Graph.Nodes.Fabricator
{
    [Serializable]
    [GraphContextMenuItem("Fabricator", "Factory")]
    public class Factory : AbstractFabricatorNode
    {
        [SerializeField] private string factoryName = "";
        [SerializeField] private Resource product = new Resource();

        [SerializeField] private float totalWeigth;
        [SerializeField] private float totalPollution;
        [SerializeField] private TimeSpan totalProductionTime = TimeSpan.Zero;

        private readonly OutputSocket productOutputSocket;
        private readonly OutputSocket byProductOutputSocket;
        private Rect tmpRect;

        private const int LeftFieldStart = 10;
        private const int RightFieldStart = 160;
        private const int FieldWidth = 120;

        public Factory(int _id, Graph _parent) : base(_id, _parent)
        {
            productOutputSocket = new OutputSocket(this, typeof(IProductConnection));
            Sockets.Add(productOutputSocket);

            byProductOutputSocket = new OutputSocket(this, typeof(IByProductConnection));
            Sockets.Add(byProductOutputSocket);

            Width = 320;
            Height = 460;
        }

        // Update is called once per frame
        protected override void OnGUI()
        {
            var guiStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal = {textColor = UnityEngine.Color.white}
            };

            tmpRect.Set(260, 0, 50, 15);
            GUI.Label(tmpRect, new GUIContent("Product"), guiStyle);

            tmpRect.Set(240, 20, 50, 15);
            GUI.Label(tmpRect, new GUIContent("By-product"), guiStyle);

            SetInputs(guiStyle);
            SetProducerName(guiStyle);
            SetProductName(guiStyle);
            SetByProductName(guiStyle);
            SetVolume(guiStyle);
            SetByVolume(guiStyle);
            SetPollution(guiStyle);
            SetEnergyConsumption(guiStyle);
            SetProductionTime(guiStyle);
            SetSummary(guiStyle);
            SetAddInput();
        }

        private void SetInputs(GUIStyle _guiStyle)
        {
            var yOffset = 0;

            var productIngredients = product.Ingredients;

            foreach (var ingredient in productIngredients)
            {
                tmpRect.Set(5, yOffset, 15, 15);

                if (GUI.Button(tmpRect, "x"))
                {
                    Sockets.Remove(ingredient.InputSocket);
                    productIngredients.Remove(ingredient);
                    return;
                }

                var tmp = ingredient.RequiredAmount.ToString();
                var text = tmp == "0" ? "" : tmp;

                tmpRect.Set(25, yOffset, 70, 15);
                text = GUI.TextField(tmpRect, text);

                try
                {
                    ingredient.RequiredAmount = Convert.ToInt32(text);
                }
                catch (FormatException e)
                {
                    ingredient.RequiredAmount = 0;
                }

                tmpRect.Set(100, yOffset, 70, 15);
                ingredient.Waste = GUI.HorizontalSlider(tmpRect, ingredient.Waste, 0f, 100.0f);

                tmpRect.Set(175, yOffset, 30, 15);
                GUI.Label(tmpRect, new GUIContent(ingredient.Waste.ToString("N2")), _guiStyle);

                yOffset += 20;
            }
        }

        private void SetProducerName(GUIStyle _guiStyle)
        {
            tmpRect.Set(LeftFieldStart, 220, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("Factory name:"), _guiStyle);

            tmpRect.Set(LeftFieldStart, 235, FieldWidth, 15);
            factoryName = GUI.TextField(tmpRect, factoryName);
        }

        private void SetProductName(GUIStyle _guiStyle)
        {
            tmpRect.Set(LeftFieldStart, 255, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("Product name:"), _guiStyle);

            tmpRect.Set(LeftFieldStart, 270, FieldWidth, 15);
            product.Name = GUI.TextField(tmpRect, product.Name);
        }

        private void SetByProductName(GUIStyle _guiStyle)
        {
            tmpRect.Set(LeftFieldStart, 290, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("By-product name:"), _guiStyle);

            tmpRect.Set(LeftFieldStart, 305, FieldWidth, 15);
            product.ByProduct.Name = GUI.TextField(tmpRect, product.ByProduct.Name);
        }

        private void SetVolume(GUIStyle _guiStyle)
        {
            tmpRect.Set(LeftFieldStart, 325, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("Product volume:"), _guiStyle);

            var tmpVolume = Math.Abs(product.Volume) < 0.001 ? "" : product.Volume.ToString("N2");

            tmpRect.Set(LeftFieldStart, 340, FieldWidth, 15);
            tmpVolume = GUI.TextField(tmpRect, tmpVolume);

            try
            {
                product.Volume = Convert.ToSingle(tmpVolume);
            }
            catch (FormatException e)
            {
                product.Volume = 0.0f;
            }
        }

        private void SetByVolume(GUIStyle _guiStyle)
        {
            tmpRect.Set(RightFieldStart, 325, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("By-product volume:"), _guiStyle);

            var tmpVolume = Math.Abs(product.ByProduct.Volume) < 0.001 ? "" : product.ByProduct.Volume.ToString("N2");

            tmpRect.Set(RightFieldStart, 340, FieldWidth, 15);
            tmpVolume = GUI.TextField(tmpRect, tmpVolume);

            try
            {
                product.ByProduct.Volume = Convert.ToSingle(tmpVolume);
            }
            catch (FormatException e)
            {
                product.ByProduct.Volume = 0.0f;
            }
        }

        private void SetPollution(GUIStyle _guiStyle)
        {
            tmpRect.Set(RightFieldStart, 220, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("Pollution:"), _guiStyle);

            var tmpPolution = Math.Abs(product.Pollution) < 0.001 ? "" : product.Pollution.ToString("N2");
            tmpRect.Set(RightFieldStart, 235, FieldWidth, 15);
            tmpPolution = GUI.TextField(tmpRect, tmpPolution);

            try
            {
                product.Pollution = Convert.ToSingle(tmpPolution);
            }
            catch (FormatException e)
            {
                product.Pollution = 0.0f;
            }
        }

        private void SetEnergyConsumption(GUIStyle _guiStyle)
        {
            tmpRect.Set(RightFieldStart, 255, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("Energy:"), _guiStyle);

            var tmpEnergy = Math.Abs(product.Energy) < 0.001 ? "" : product.Energy.ToString("N2");
            tmpRect.Set(RightFieldStart, 270, FieldWidth, 15);
            tmpEnergy = GUI.TextField(tmpRect, tmpEnergy);

            try
            {
                product.Energy = Convert.ToSingle(tmpEnergy);
            }
            catch (FormatException e)
            {
                product.Energy = 0.0f;
            }
        }

        private void SetProductionTime(GUIStyle _guiStyle)
        {
            tmpRect.Set(RightFieldStart, 290, FieldWidth, 10);
            GUI.Label(tmpRect, new GUIContent("Production time:"), _guiStyle);

            var tmpTime = Math.Abs(product.WorkTime.TotalSeconds) < 0.1 ? "" : product.WorkTime.TotalSeconds.ToString();
            tmpRect.Set(RightFieldStart, 305, FieldWidth, 15);
            tmpTime = GUI.TextField(tmpRect, tmpTime);

            try
            {
                product.WorkTime = TimeSpan.FromSeconds(Convert.ToDouble(tmpTime));
            }
            catch (OverflowException e)
            {
            }
            catch (FormatException e)
            {
            }
            catch (ArgumentException e)
            {
            }
        }

        private void SetSummary(GUIStyle _guiStyle)
        {
            var tmpWeigth = Math.Abs(totalWeigth) < 0.001 ? "" : totalWeigth.ToString("N2");
            tmpRect.Set(LeftFieldStart, 370, 200, 10);
            GUI.Label(tmpRect, new GUIContent("Total weigth: " + tmpWeigth), _guiStyle);

            var tmpPollution = Math.Abs(totalPollution) < 0.001 ? "" : totalPollution.ToString("N2");
            tmpRect.Set(LeftFieldStart, 385, 200, 10);
            GUI.Label(tmpRect, new GUIContent("Total pollution: " + tmpPollution), _guiStyle);

            tmpRect.Set(LeftFieldStart, 400, 200, 10);
            GUI.Label(tmpRect, new GUIContent("Total production time: " + totalProductionTime), _guiStyle);

            tmpRect.Set(240, 60, 70, 20);
            if (GUI.Button(tmpRect, "Update summary"))
            {
                var sum = new Resource();

                foreach (var ingredient in product.Ingredients)
                {
                    var inputResource = GetInputResource(ingredient.InputSocket, new Request());
                    
                    if(inputResource == null)
                        continue;
                    
                    sum.Add(inputResource);
                }

                totalWeigth = sum.Weigth + product.Weigth;
                totalPollution = sum.Pollution + product.Pollution;
                totalProductionTime = sum.WorkTime + product.WorkTime;
            }
        }

        private void SetAddInput()
        {
            tmpRect.Set(240, 40, 70, 20);
            if (!GUI.Button(tmpRect, "Add input"))
                return;

            if (product.Ingredients.Count >= 10)
                return;

            var inputSocket = new InputSocket(this, typeof(IFabricator));
            product.Ingredients.Add(new Resource.Ingredient(inputSocket));
            Sockets.Add(inputSocket);
        }

        public override void Update()
        {
        }

        public override Resource GetResource(OutputSocket _outSocket, Request _request)
        {
            var sum = new Resource();

            foreach (var ingredient in product.Ingredients)
            {
                var inputResource = GetInputResource(ingredient.InputSocket, _request);
                sum.Add(inputResource);
            }

            return sum;
        }
    }
}