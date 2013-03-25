using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class UFTObjectOnScene {	
	public Component component;	
	public List<FieldInfo> propertyList;
	public UFTObjectOnScene(Component component){
		this.component = component;
		propertyList = new List<FieldInfo>();	
	}
	
	public void addProperty(FieldInfo property){
		propertyList.Add(property);
	}
}
