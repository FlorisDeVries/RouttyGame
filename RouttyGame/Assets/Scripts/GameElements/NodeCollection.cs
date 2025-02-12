using System;
using _Common.BaseClasses;
using _Common.Unity;
using GameElements.Nodes;
using UnityEngine;

namespace GameElements
{
    [CreateAssetMenu(fileName = "Node Collection", menuName = "Project/GameElements/Node Collection")]
    public class NodeCollection : ASingletonResource<NodeCollection>
    {
        protected override string ResourcePath => "GameElements/Node Collection";
        
        public ShapeToSpriteDictionary ShapeToSpriteDictionary;
        public ColorToColorDictionary ColorToColorDictionary;
    }
    
    [Serializable]
    public class ShapeToSpriteDictionary : UnitySerializedDictionary<NodeShape, Sprite> { }
        
    [Serializable]
    public class ColorToColorDictionary : UnitySerializedDictionary<NodeColor, Color> { }
}