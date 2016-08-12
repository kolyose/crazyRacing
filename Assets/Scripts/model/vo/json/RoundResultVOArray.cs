using System;
using System.Collections.Generic;

/*
 * Just a wrapper class for RoundResultVO for proper JSON parsing
 */

[Serializable]
public class RoundResultVOArray
{
    public RoundResultVO[] results;
}
