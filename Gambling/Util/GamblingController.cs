using GameNetcodeStuff;
using LC_API.Data;
using LC_API.GameInterfaceAPI;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using LC_API.GameInterfaceAPI.Features;
using LC_API.ServerAPI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Gambling.Util;

public class GamblingController : MonoBehaviour
{
    private PlayerControllerB _playerControllerB;
    [FormerlySerializedAs("_domainUser")] public ulong domainUser = 99999999L;
    [FormerlySerializedAs("_domainActive")] public bool domainActive;

    private void Awake()
    {
        _playerControllerB = GetComponent<PlayerControllerB>();
        LC_API.GameInterfaceAPI.Events.Handlers.Player.Hurting += SillyBilly;
        LC_API.GameInterfaceAPI.Events.Handlers.Player.Dying += SillyBob;
        GameState.WentIntoOrbit += ResetGambitOnOrbit;
    }
    
    void OnDestroy()
    {
        LC_API.GameInterfaceAPI.Events.Handlers.Player.Hurting -= SillyBilly;
        LC_API.GameInterfaceAPI.Events.Handlers.Player.Dying -= SillyBob;
        GameState.WentIntoOrbit -= ResetGambitOnOrbit;
    }

    public void SillyBilly(HurtingEventArgs ev)
    {
        if (domainActive && ev.Player.ClientId == domainUser && !shouldSkip(ev.CauseOfDeath))
        {
            GamblingPlugin.Instance.Log.LogInfo("NO PAIN FOR ME BABY!");
            ev.IsAllowed = false;
        }
    }

    public void SillyBob(DyingEventArgs ev)
    {
        if (domainActive && ev.Player.ClientId == domainUser && !shouldSkip(ev.CauseOfDeath))
        {
            GamblingPlugin.Instance.Log.LogInfo("NO DEATH FOR ME BABY!");
            ev.IsAllowed = false;
        }
    }

    private bool shouldSkip(CauseOfDeath causeOfDeath)
    {
        return causeOfDeath == CauseOfDeath.Abandoned;
    }
    
    private void Update()
    {
        if (!_playerControllerB.IsOwner ||!_playerControllerB.isPlayerControlled) return;
        
        if (domainActive)
        {
            _playerControllerB.sprintMeter = 100f;
        }
        
        if (GamblingPlugin.InputActionsInstance.ExpansionKey.triggered && !domainActive && GameState.ShipState == ShipState.OnMoon)
        {
            GamblingPlugin.Instance.Log.LogInfo(domainActive + " - " + domainUser);
            if (Random.Range(0f, 1f) >= 0.85 || GamblingPlugin.Instance.shouldDebug)
            {
                if (_playerControllerB.isPlayerDead) return;
                
                domainActive = true;
                
                LC_API.GameInterfaceAPI.GameTips.ShowTip("IDLE DEATH GAMBLE: Start", "JACKPOT BABY!!!!!!!!!");
                
                Player player = Player.Get(_playerControllerB);
                GamblingPlugin.Instance.Log.LogInfo(player.ClientId);
                
                domainUser = player.ClientId;
                
                player.SprintMeter = float.PositiveInfinity;
                
                AudioSource source =
                    gameObject.AddComponent<AudioSource>();
                source.clip = GamblingPlugin.Instance.gambit;
                source.Play();
                
                Invoke(nameof(ResetGambit), 251f);

                LC_API.GameInterfaceAPI.Features.Item.CreateAndGiveItem("shovel", player, false);
            }
            else
            {
                Player.Get(_playerControllerB).Hurt(50);
            }
        }
    }

    [ServerRpc]
    public void PlayAudio()
    {
        AudioSource source =
            gameObject.AddComponent<AudioSource>();
        source.clip = GamblingPlugin.Instance.gambit;
        source.Play();
                
        Invoke(nameof(ResetGambit), 251f);
    }

    public void ResetGambitOnOrbit()
    {
        CancelInvoke(nameof(ResetGambit));
        ResetGambit();
    }
    
    public void ResetGambit()
    {
        if (!domainActive) return;
        
        LC_API.GameInterfaceAPI.GameTips.ShowTip("IDLE DEATH GAMBLE: End", "Maybe round 2?");
        domainActive = false;
        domainUser = 99999999L;
    }
}