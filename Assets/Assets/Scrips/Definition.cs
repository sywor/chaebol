using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DefinitionConverter : JsonConverter
{
    public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
    {
        var jsonObject = JObject.Load(_reader);
        var displayName = jsonObject["DisplayName"].Value<string>();
        var model = jsonObject["Model"].Value<string>();
        var texture = jsonObject["Texture"].Value<string>();

        var snapPoints = jsonObject["SnapPoints"].Select(_jSnapPoint => _serializer.Deserialize<SnapPoint>(_jSnapPoint.CreateReader())).ToList();

        return new Definition(displayName, model, texture, snapPoints);
    }

    public override bool CanConvert(Type _objectType)
    {
        return typeof(Definition) == _objectType;
    }

    public override bool CanWrite
    {
        get { return false; }
    }
}

public class SnapPointConverter : JsonConverter
{
    public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
    {
        var jsonObject = JObject.Load(_reader);
        var name = jsonObject["Name"].Value<string>();
        var positionArr = jsonObject["Position"].Values<float>().ToArray();
        var rotationArr = jsonObject["Rotation"].Values<float>().ToArray();
        var connectsTo = jsonObject["ConnectsTo"].Values<string>().ToList();

        var position = new Vector3(positionArr[0], positionArr[1], positionArr[2]);
        var rotation = new Vector3(rotationArr[0], rotationArr[1], rotationArr[2]);

        return new SnapPoint(name, position, rotation, connectsTo);
    }

    public override bool CanConvert(Type _objectType)
    {
        return typeof(SnapPoint) == _objectType;
    }

    public override bool CanWrite
    {
        get { return false; }
    }
}

public class Definition
{
    public string DisplayName { get; private set; }
    public string Model { get; private set; }
    public string Texture { get; private set; }
    public List<SnapPoint> SnapPoints { get; private set; }

    public Definition(string _displayName, string _model, string _texture, List<SnapPoint> _snapPoints)
    {
        DisplayName = _displayName;
        Model = _model;
        Texture = _texture;
        SnapPoints = _snapPoints;
    }

    public override string ToString()
    {
        return "Model: " + Model + " Texture: " + Texture + " SnapPoints: " + SnapPoints.PrettyPrint();
    }
}

public class SnapPoint
{
    public string Name { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 Rotation { get; private set; }
    public List<string> ConnectsTo { get; private set; }

    public SnapPoint(string _name, Vector3 _position, Vector3 _rotation, List<string> _connectsTo)
    {
        Name = _name;
        Position = _position;
        Rotation = _rotation;
        ConnectsTo = _connectsTo;
    }

    public override string ToString()
    {
        return "Name: " + Name + " Position: " + Position.PrettyPrint() + " ConnectsTo: " + ConnectsTo.PrettyPrint();
    }
}
