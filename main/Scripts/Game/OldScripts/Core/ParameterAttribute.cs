///////////////////////////////////////////////////////////////////////
//
//  ParameterAttribute.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
public class ParameterAttribute : System.Attribute
{
	public string Name
	{
		get; set;
	}	

	public string Category
	{
		get; set;
	}

	public List<string> FormulaVariables
	{
		get; set;
	}

	public ParameterAttribute(string name, string category = "")
	{
		Name = name;
		Category = category;
	}
}