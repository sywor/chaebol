using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SnapPointConverter : JsonConverter
{
    public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader _reader,
                                    Type _objectType,
                                    object _existingValue,
                                    JsonSerializer _serializer)
    {
        var jsonObject = JObject.Load(_reader);
        var name = jsonObject["Name"].Value<string>();
        var positionArr = jsonObject["Position"].Values<float>().ToArray();
        var rotation = jsonObject["Rotation"].Value<float>();
        var connectsTo = jsonObject["ConnectsTo"].Values<string>().ToList();

        var position = new Vector2(positionArr[0], positionArr[1]);

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

    public Definition(string DisplayName, string Model, string Texture, List<SnapPoint> SnapPoints)
    {
        if (DisplayName == null) throw new ArgumentNullException("DisplayName");
        if (Model == null) throw new ArgumentNullException("Model");
        if (Texture == null) throw new ArgumentNullException("Texture");
        if (SnapPoints == null) throw new ArgumentNullException("SnapPoints");
        
        this.DisplayName = DisplayName;
        this.Model = Model;
        this.Texture = Texture;
        this.SnapPoints = SnapPoints;
    }

    public override string ToString()
    {
        return string.Format("DisplayName: {0}, Model: {1}, Texture: {2}, SnapPoints: {3}", DisplayName, Model, Texture, SnapPoints);
    }

    protected bool Equals(Definition _other)
    {
        return string.Equals(DisplayName, _other.DisplayName) &&
               string.Equals(Model, _other.Model) &&
               string.Equals(Texture, _other.Texture) &&
               SnapPoints.SequenceEqual(_other.SnapPoints);
    }

    public override bool Equals(object _obj)
    {
        if (ReferenceEquals(null, _obj)) return false;
        if (ReferenceEquals(this, _obj)) return true;
        if (_obj.GetType() != GetType()) return false;
        return Equals((Definition) _obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = DisplayName.GetHashCode();
            hashCode = (hashCode * 397) ^ Model.GetHashCode();
            hashCode = (hashCode * 397) ^ Texture.GetHashCode();
            hashCode = (hashCode * 397) ^ SnapPoints.GetHashCode();
            return hashCode;
        }
    }
}

public class SnapPoint
{
    public string Name { get; private set; }
    public Vector2 Position { get; private set; }
    public float Rotation { get; private set; }
    public List<string> ConnectsTo { get; private set; }

    public SnapPoint(string _name, Vector2 _position, float _rotation, List<string> _connectsTo)
    {
        Name = _name;
        Position = _position;
        Rotation = _rotation;
        ConnectsTo = _connectsTo;
    }

    public override string ToString()
    {
        return string.Format("Name: {0}, Position: {1}, Rotation: {2}, ConnectsTo: {3}", Name, Position, Rotation, ConnectsTo);
    }

    protected bool Equals(SnapPoint _other)
    {
        return string.Equals(Name, _other.Name) &&
               Position.Equals(_other.Position) &&
               Rotation.Equals(_other.Rotation) &&
               ConnectsTo.SequenceEqual(_other.ConnectsTo);
    }

    public override bool Equals(object _obj)
    {
        if (ReferenceEquals(null, _obj))
            return false;
        if (ReferenceEquals(this, _obj))
            return true;
        if (_obj.GetType() != GetType())
            return false;
        return Equals((SnapPoint) _obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Position.GetHashCode();
            hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
            hashCode = (hashCode * 397) ^ ConnectsTo.GetHashCode();
            return hashCode;
        }
    }
}
