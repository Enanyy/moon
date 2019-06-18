using System;
using UnityEngine;
using Random = UnityEngine.Random;

    public enum ElementType
    {
        Type1,
        Type2,
        Type3,

        Type4,

    }
    [Serializable]
	internal class MyTreeElement : TreeElement
	{
		public float floatValue1, floatValue2, floatValue3;
		public Material material;
		public string text = "";
        public ElementType type = ElementType.Type1;
        public bool enabled;

		public MyTreeElement (string name, int depth, int id) : base (name, depth, id)
		{
			floatValue1 = Random.value;
			floatValue2 = Random.value;
			floatValue3 = Random.value;
			enabled = true;
		}
	}

