using UnityEngine;  //Needed for most Unity Enginer manipulations: Vectors, GameObjects, Audio, etc.
using System.IO;    //For data read/write methods
using System;    //For data read/write methods
using System.Collections.Generic;   //Working with Lists and Collections
using System.Linq;   //More advanced manipulation of lists/collections
using Harmony;
using ReikaKalseki.FortressCore;

namespace ReikaKalseki.ChromedLPTRebalance
{
  public class ChromedLPTRebalanceMod : FCoreMod
  {
    public const string MOD_KEY = "ReikaKalseki.ChromedLPTRebalance";
    public const string CUBE_KEY = "ReikaKalseki.ChromedLPTRebalance_Key";
    
    private const float GREEN_BASE_PPS = 10;
    private const float BLUE_BASE_PPS = 40;
    private const float PURPLE_BASE_PPS = 320;
    
    private const float CHROMED_FACTOR = 5F;
    
    private const float CHROMED_GREEN_PPS = 180;
    private const float CHROMED_BLUE_PPS = 250;
    private const float CHROMED_PURPLE_PPS = 2000; //1600 in vanilla
    
    public ChromedLPTRebalanceMod() : base("ChromedLPTRebalance") {
    	
    }

    public override ModRegistrationData Register()
    {
        ModRegistrationData registrationData = new ModRegistrationData();
        //registrationData.RegisterEntityHandler(MOD_KEY);
        /*
        TerrainDataEntry entry;
        TerrainDataValueEntry valueEntry;
        TerrainData.GetCubeByKey(CUBE_KEY, out entry, out valueEntry);
        if (entry != null)
          ModCubeType = entry.CubeType;
         */        
        var harmony = HarmonyInstance.Create("ReikaKalseki.ChromedLPTRebalance");
        HarmonyInstance.DEBUG = true;
        FileLog.Log("Ran mod register, started harmony (harmony log)");
        FUtil.log("Ran mod register, started harmony");
        try {
        	harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }
        catch (Exception e) {
			FileLog.Log("Caught exception when running patcher!");
			FileLog.Log(e.Message);
			FileLog.Log(e.StackTrace);
			FileLog.Log(e.ToString());
        }
        
        return registrationData;
    }
    
    private static float calculateNeededBonus(float baseGen, float target) {
    	return (target-baseGen);
    }
    
    public static float getPPSBonus(float original, LaserPowerTransmitter lpt, int tier) {
    	if (tier != 10) //not chromed
    		return original;
    	if (lpt.mValue > 2) //not one of the base three
    		return original;
    	switch(lpt.mValue) {
    		case 0: //green
    			return calculateNeededBonus(GREEN_BASE_PPS, CHROMED_GREEN_PPS);
    		case 1: //blue
    			return calculateNeededBonus(BLUE_BASE_PPS, CHROMED_BLUE_PPS);
    		case 2: //purple
    			return calculateNeededBonus(PURPLE_BASE_PPS, CHROMED_PURPLE_PPS);
    		default:
    			return original;
    	}
    }
    
    public static float getPPSMultiplier(float original, LaserPowerTransmitter lpt, int tier) {
    	if (tier != 10) //not chromed
    		return original;
    	if (lpt.mValue > 2) //not one of the base three
    		return original;
    	return 0; //so as not to cause issues when combined with bonus
    }

  }
}
