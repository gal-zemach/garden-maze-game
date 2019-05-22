﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{	
	public abstract float HorizontalAxis();
	
	public abstract float VerticalAxis();
}
