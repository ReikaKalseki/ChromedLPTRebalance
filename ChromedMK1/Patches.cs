/*
 * Created by SharpDevelop.
 * User: Reika
 * Date: 04/11/2019
 * Time: 11:28 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;    //For data read/write methods
using System.Collections;   //Working with Lists and Collections
using System.Collections.Generic;   //Working with Lists and Collections
using System.Linq;   //More advanced manipulation of lists/collections
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using Harmony;
using UnityEngine;  //Needed for most Unity Enginer manipulations: Vectors, GameObjects, Audio, etc.

namespace ReikaKalseki.ChromedLPTRebalance {
	
	[HarmonyPatch(typeof(LaserPowerTransmitter))]
	[HarmonyPatch("GetBonus")]
	public static class BonusPatch {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			return Lib.patch(instructions, "getPPSBonus");
		}
	}
	
	[HarmonyPatch(typeof(LaserPowerTransmitter))]
	[HarmonyPatch("GetMultiplier")]
	public static class FactorPatch {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			return Lib.patch(instructions, "getPPSMultiplier");
		}
	}
	
	static class Lib {
	
		internal static IEnumerable<CodeInstruction> patch(IEnumerable<CodeInstruction> li, string call) {
			List<CodeInstruction> codes = new List<CodeInstruction>(li);
			try {
				int loc = InstructionHandlers.getLastInstructionBefore(codes, codes.Count, OpCodes.Ret);
				FileLog.Log("Running patch, which found anchor "+InstructionHandlers.toString(codes, loc));
				List<CodeInstruction> inject = new List<CodeInstruction>();
				inject.Add(new CodeInstruction(OpCodes.Ldarg_0)); //this
				inject.Add(new CodeInstruction(OpCodes.Ldarg_1)); //int tier
				inject.Add(InstructionHandlers.createMethodCall("ReikaKalseki.ChromedLPTRebalance.ChromedLPTRebalanceMod", call, false, typeof(float), typeof(LaserPowerTransmitter), typeof(int)));
				FileLog.Log("Injecting "+inject.Count+" instructions: "+InstructionHandlers.toString(inject));
				codes.InsertRange(loc, inject);
				FileLog.Log("Done patch "+new StackFrame(1).GetMethod().Name);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+new StackFrame(1).GetMethod().Name+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
		
	}
}
