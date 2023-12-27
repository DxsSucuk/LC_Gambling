using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace Gambling.Util;

public class GamblingInput : LcInputActions
{
    
    [InputAction("<Keyboard>/g", Name = "Gamble")]
    public InputAction ExpansionKey { get; set; }
}