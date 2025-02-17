using System;
using System.Collections.Generic;
using _Common.BaseClasses;
using _Common.Extensions;
using _Common.Unity;
using GameElements.Nodes;
using UnityEngine;

namespace GameElements.Resources
{
    [CreateAssetMenu(fileName = "Channel Collection", menuName = "Project/GameElements/Channel Collection")]
    public class ChannelCollection : ASingletonResource<ChannelCollection>
    {
        protected override string ResourcePath => "GameElements/Channel Collection";
        
        public List<Sprite> ChannelSprites;
        public List<Sprite> ErpSprites;
        
        public void ShuffleChannels()
        {
            ChannelSprites.Shuffle();
        }
        
        public void ShuffleErps()
        {
            ErpSprites.Shuffle();
        }
    }
}