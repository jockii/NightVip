using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Listeners;
using System;
using System.Data;
using System.Numerics;
using System.Text.Json.Serialization;
using static CounterStrikeSharp.API.Core.Listeners;

namespace NightVip;

public class NightVipConfig : BasePluginConfig
{
    [JsonPropertyName("PluginStartTime")] public string PluginStartTime { get; set; } = "20:00:00";
    [JsonPropertyName("PluginEndTime")] public string PluginEndTime { get; set; } = "06:00:00";
    [JsonPropertyName("ScoreBoardTag")] public string ScoreBoardTag { get; set; } = "[NightVip]";
    [JsonPropertyName("UseScoreBoardTag")] public bool UseScoreBoardTag { get; set; } = true;
    [JsonPropertyName("Health")] public int Health { get; set; } = 100;
    [JsonPropertyName("Armor")] public int Armor { get; set; } = 100;
    [JsonPropertyName("Money")] public int Money { get; set; } = 16000;
    [JsonPropertyName("Gravity")] public float Gravity { get; set; } = 1.0f;
    [JsonPropertyName("AutoGiveVip")] public bool AutoGiveVip { get; set; } = true;

    //[JsonPropertyName("EnableBunnyHop")] public bool EnableBunnyHop { get; set; } = true;

    [JsonPropertyName("GiveHealthShot")] public bool GiveHealthShot { get; set; } = true;
    [JsonPropertyName("GivePlayerItem")] public bool GivePlayerItem { get; set; } = true;
    [JsonPropertyName("WeaponsList")] public List<string?> WeaponsList { get; set; } = new List<string?> { "weapon_ak47", "weapon_deagle" };
}

[MinimumApiVersion(116)]
public class NightVip : BasePlugin, IPluginConfig<NightVipConfig>
{
    public override string ModuleName => "NightVip";
    public override string ModuleVersion => "v1.2.0";
    public override string ModuleAuthor => "jockii";

    public List<int?> _vips = new List<int?>();
    public int Round = 0;

    public NightVipConfig Config { get; set; } = new NightVipConfig();
    public void OnConfigParsed(NightVipConfig config)
    {
        Config = config;
    }
    public override void Load(bool hotReload)
    {
        if(Config.AutoGiveVip)
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);

        Console.WriteLine($"##########################################\n" +
            $"Plugin: {ModuleName} {ModuleVersion}\nAuthor: {ModuleAuthor}\nInfo: https://github.com/jockii\nSupport me: https://www.buymeacoffee.com/jockii\n" +
            $"##########################################");

        RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            var pl = @event.Userid;
            _vips.Remove(pl.UserId);

            return HookResult.Continue;
        });
    }

    private string TimeNow(string time)
    {
        int i = time.IndexOf('.');

        if (i != -1)
        {
            time = time.Substring(0, i);
        }
        return time;
    }

    //private void SetupBhop(CCSPlayerController player)
    //{
    //    if (player == null || !player.IsValid || player.IsBot || player.IsHLTV || !player.PlayerPawn.IsValid)
    //        return;
    //    else
    //    {
    //        Server.ExecuteCommand("sv_cheats true");
    //        player.ExecuteClientCommand("sv_autobunnyhopping true");
    //        Server.ExecuteCommand("sv_cheats false");
    //    }
    //}

    private void GiveWeaponItem(CCSPlayerController pl, string item)
    {
        if (item == null || item == "") return;
        else pl.GiveNamedItem(item);
    }

    private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV || !player.PlayerPawn.IsValid) 
            return HookResult.Continue;
        else
            _vips.Add(player.UserId);

        return HookResult.Continue;
    }

    [ConsoleCommand("nightvip", "Add players in vips list")]
    [ConsoleCommand("nvip", "Add players in vips list")]
    [ConsoleCommand("nv", "Add players in vips list")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnCommandNightVip(CCSPlayerController? controller, CommandInfo info)
    {
        if (controller == null || !controller.IsValid || !controller.Pawn.IsValid || controller.IsBot || controller.IsHLTV) return;

        TimeSpan.TryParse(Config.PluginStartTime, out TimeSpan start_time);
        TimeSpan.TryParse(Config.PluginEndTime, out TimeSpan end_time);
        TimeSpan.TryParse(TimeNow(DateTime.Now.TimeOfDay.ToString()), out TimeSpan current_time);

        if (start_time <= current_time || current_time < end_time)
        {
            if (_vips.Contains(controller.UserId))
                controller.PrintToChat($" {ChatColors.LightPurple}{Config.ScoreBoardTag} {ChatColors.LightRed}You already have status NightVip");

            if (!_vips.Contains(controller.UserId))
            {
                _vips.Add(controller.UserId);

                Server.NextFrame(() =>
                {
                    AddTimer(1.0f, () =>
                    {
                        controller.Pawn.Value!.Health = Config.Health;
                        controller.PawnArmor = Config.Armor;
                        controller.PawnHasHelmet = true;
                        controller.InGameMoneyServices!.Account = Config.Money;
                        controller.Pawn.Value.GravityScale = Config.Gravity;

                        if (Config.GiveHealthShot)
                            controller.GiveNamedItem("weapon_healthshot");

                        if (Config.UseScoreBoardTag)
                            controller.Clan = Config.ScoreBoardTag;
                    });
                });
                controller.PrintToChat($" {ChatColors.LightPurple}{Config.ScoreBoardTag} {ChatColors.Olive}Perfectly. Now you have privileges!");
            }
        }
        else
            controller.PrintToChat($" {ChatColors.LightPurple}{Config.ScoreBoardTag} {ChatColors.LightRed}In general, it is not yet night for a night vip");
    }

    [GameEventHandler(mode: HookMode.Post)]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var pl = @event.Userid;

        TimeSpan.TryParse(Config.PluginStartTime, out TimeSpan start_time);
        TimeSpan.TryParse(Config.PluginEndTime, out TimeSpan end_time);
        TimeSpan.TryParse(TimeNow(DateTime.Now.TimeOfDay.ToString()), out TimeSpan current_time);

        if (start_time <= current_time || current_time < end_time)
        {
            if (pl == null || !pl.IsValid || !pl.Pawn.IsValid || pl.IsBot || pl.IsHLTV) return HookResult.Continue;

            if (_vips.Contains(pl.UserId))
            {
                AddTimer(1.0f, () =>
                {
                    if (Config.GivePlayerItem)
                    {
                        for (int i = 0; i < Config.WeaponsList.Count; i++)
                        {
                            if (i > Config.WeaponsList.Count) break;
                            GiveWeaponItem(pl, Config.WeaponsList[i]!);
                        }
                    }

                    pl.Pawn.Value!.Health = Config.Health;                  // ok
                    pl.PlayerPawn.Value!.ArmorValue = Config.Armor;         // ok
                    pl.PawnHasHelmet = true;                                // ok
                    pl.InGameMoneyServices!.Account = Config.Money;         // ok
                    pl.Pawn.Value.GravityScale = Config.Gravity;            // ok

                    if (Config.GiveHealthShot)
                        pl.GiveNamedItem("weapon_healthshot");              // vrodi ok

                    if (Config.UseScoreBoardTag)
                        pl.Clan = Config.ScoreBoardTag;                     // ok

                    //if (Config.EnableBunnyHop)
                    //    SetupBhop(pl);

                });
            }

            if (!_vips.Contains(pl.UserId)) return HookResult.Continue;
        }
        else
        {
            return HookResult.Continue;
        }

        return HookResult.Continue;
    }
}