using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

[CreateAssetMenu]
public class TypeRegistry : ScriptableObject
{
    private struct Resource
    {
        private readonly FileInfo definition;
        private readonly List<FileInfo> resources;

        public Resource(FileInfo _definition, List<FileInfo> _resources)
        {
            definition = _definition;
            resources = _resources;
        }

        public FileInfo Definition
        {
            get { return definition; }
        }

        public FileInfo GetResourceFileInfo(string _name)
        {
            return resources.SingleOrDefault(_r => _r.Name == _name);
        }

        public override string ToString()
        {
            return "Definition: " + definition + " Resources: " + resources.PrettyPrint();
        }
    }

    private static TypeRegistry instance;
    public static TypeRegistry Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TypeRegistry>();
            }

            if (instance == null)
            {
                instance = CreateInstance<TypeRegistry>();
            }

            return instance;
        }
    }

    private readonly Dictionary<string, IPlaceable> lookUpPlaceables = new Dictionary<string, IPlaceable>();
    private string defintionsRoot;

    public void Init()
    {
        Debug.Log("TypeRegistry: Init");
        defintionsRoot = (Application.dataPath + "/Assets/Resources/Definitions").Replace("/", @"\");

        var resourcesDirectoryInfo = new DirectoryInfo(defintionsRoot);
        var resources = new List<Resource>();

        CrawlDirectory(resourcesDirectoryInfo, resources);

        foreach (var resource in resources)
        {
            var jsonDefinition = File.ReadAllText(resource.Definition.ToString());
            CreatePlaceable(resource, JsonConvert.DeserializeObject<Definition>(jsonDefinition, new DefinitionConverter(), new SnapPointConverter()));
        }
    }

    private static void CrawlDirectory(DirectoryInfo _directoryInfo, List<Resource> _resources)
    {
        var fileInfos = _directoryInfo.GetFiles();

        if (fileInfos.Length > 0)
        {
            var definition = fileInfos.SingleOrDefault(_f => _f.Extension == ".json");

            if (definition != null)
            {
                var resources = fileInfos.Where(_f => _f != definition && _f.Extension != ".meta").ToList();
                _resources.Add(new Resource(definition, resources));
            }
        }

        foreach (var directoryInfo in _directoryInfo.GetDirectories())
        {
            CrawlDirectory(directoryInfo, _resources);
        }
    }

    private void CreatePlaceable(Resource _resource, Definition _definition)
    {
        var key = _resource.Definition.Name.Split('.').First();
        var placable = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placable.name = key;

        var meshPath = TrimPath(_resource.GetResourceFileInfo(_definition.Model));
        var mesh = Resources.Load<Mesh>("Definitions" + meshPath);
        placable.GetComponent<MeshFilter>().mesh = mesh;

        var materialPath = _resource.GetResourceFileInfo(_definition.Texture);
        var texture = new Texture2D(2, 2);
        texture.LoadImage(File.ReadAllBytes(materialPath.FullName));
        placable.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);

        placable.AddComponent<PlaceDownTrigger>();

        lookUpPlaceables.Add(key, ScriptedPlacable.Create(placable));
    }

    private string TrimPath(FileInfo _fileInfo)
    {
        var fileInfoFullName = _fileInfo.FullName;
        var trimPath = fileInfoFullName.Replace(defintionsRoot, "");
        trimPath = trimPath.Replace(_fileInfo.Extension, "");
        return trimPath;
    }
}
