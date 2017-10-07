using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;

public class DefinitionJsonTest
{
    [Test]
    public void TestSerializeMinimalDefinition()
    {
        var serializedObject = JsonConvert.SerializeObject(new Definition("displayName", "model", "texture", new List<SnapPoint>()));
        Assert.That(serializedObject, Is.EqualTo("{\"DisplayName\":\"displayName\",\"Model\":\"model\",\"Texture\":\"texture\",\"SnapPoints\":[]}"));
    }
    
    [Test]
    public void TestRequireDispayNameNotNull()
    {
        const string definitionWithoutName = "{Model: \"model\", Texture: \"texture\", SnapPoints: []}";
        Assert.That(() => JsonConvert.DeserializeObject<Definition>(definitionWithoutName), Throws.Exception);
    }
    
    [Test]
    public void TestRequireModelNotNull()
    {
        const string definitionWithoutModel = "{DisplayName: \"displayName\", Texture: \"texture\", SnapPoints: []}";
        Assert.That(() => JsonConvert.DeserializeObject<Definition>(definitionWithoutModel), Throws.Exception);
    }
    
    [Test]
    public void TestRequireTextureNotNull()
    {
        const string definitionWithoutTexture = "{DisplayName: \"displayName\", Model: \"model\", SnapPoints: []}";
        Assert.That(() => JsonConvert.DeserializeObject<Definition>(definitionWithoutTexture), Throws.Exception);
    }
    
    [Test]
    public void TestRequireSnapPointsNotNull()
    {
        const string definitionWithoutSnapPoints = "{DisplayName: \"displayName\", Model: \"model\", Texture: \"texture\"}";
        Assert.That(() => JsonConvert.DeserializeObject<Definition>(definitionWithoutSnapPoints), Throws.Exception);
    }

    [Test]
    public void TestReadDefinitionWithNoSnapPoint()
    {
        const string minimalObject = "{DisplayName: \"displayName\", Model: \"model\", Texture: \"texture\", SnapPoints: []}";
        var deserializedDefinition = JsonConvert.DeserializeObject<Definition>(minimalObject);

        var expectedDefinition = new Definition("displayName", "model", "texture", new List<SnapPoint>());
        Assert.That(deserializedDefinition, Is.EqualTo(expectedDefinition));
    }

    [Test]
    public void TestReadDefinitionWithMultipleSnapPoints()
    {
        const string representation = "{DisplayName: \"displayName\", Model: \"model\", Texture: \"texture\", SnapPoints: [\"snapPoint1\", \"snapPoint2\", \"snapPoint3\"]}";

        var snapPoint1 = new SnapPoint("name1", Vector3.zero, 0, new List<string>());
        var snapPoint2 = new SnapPoint("name2", Vector3.up, 0, new List<string>());
        var snapPoint3 = new SnapPoint("name3", Vector3.zero, 1, new List<string>());
        
        var snapPointConverterFake = new ConverterFake<SnapPoint>();
        snapPointConverterFake.PreparedResults.Add("snapPoint1", snapPoint1);
        snapPointConverterFake.PreparedResults.Add("snapPoint2", snapPoint2);
        snapPointConverterFake.PreparedResults.Add("snapPoint3", snapPoint3);
        
        var deserializedDefinition = JsonConvert.DeserializeObject<Definition>(representation, snapPointConverterFake);

        var snapPoints = new List<SnapPoint>
        {
            snapPoint1,
            snapPoint2,
            snapPoint3
        };
        var expectedDefinition = new Definition("displayName", "model", "texture", snapPoints);
        Assert.That(deserializedDefinition, Is.EqualTo(expectedDefinition));
    }

    private class ConverterFake<T> : JsonConverter
    {
        public readonly Dictionary<string, T> PreparedResults = new Dictionary<string, T>();
        
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            var value = JToken.Load(_reader).Value<string>();
            return PreparedResults[value];
        }

        public override bool CanConvert(Type _objectType)
        {
            return typeof(T) == _objectType;
        }
    }
}