using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Cvars;

namespace NightVip;

public class NightVipConfig : BasePluginConfig
{
    [JsonPropertyName("PluginStartTime")] public string PluginStartTime { get; set; } = "20:00:00";
    [JsonPropertyName("PluginEndTime")] public string PluginEndTime { get; set; } = "06:00:00";
    [JsonPropertyName("AutoGiveVip")] public bool AutoGiveVip { get; set; } = true;
    [JsonPropertyName("EnableAutoBhop")] public bool EnableAutoBhop { get; set; } = true;
    [JsonPropertyName("GiveHealthShot")] public bool GiveHealthShot { get; set; } = true;
    [JsonPropertyName("GivePlayerWeapons")] public bool GivePlayerWeapons { get; set; } = true;
    [JsonPropertyName("UseScoreBoardTag")] public bool UseScoreBoardTag { get; set; } = true;

    //[JsonPropertyName("EnableJumpCount")] public bool EnableJumpCount { get; set; } = true;

    [JsonPropertyName("ScoreBoardTag")] public string ScoreBoardTag { get; set; } = "[NightVip]";
    [JsonPropertyName("DisableVipRounds")] public string DisableVipRounds { get; set; } = "1,13";

    //[JsonPropertyName("JumpCount")] public int JumpCount { get; set; } = 2;

    [JsonPropertyName("Health")] public int Health { get; set; } = 100;
    [JsonPropertyName("Armor")] public int Armor { get; set; } = 100;
    [JsonPropertyName("Money")] public int Money { get; set; } = 16000;
    [JsonPropertyName("Gravity")] public float Gravity { get; set; } = 1.0f;
    [JsonPropertyName("WeaponsList")] public List<string?> WeaponsList { get; set; } = new List<string?> { "weapon_ak47", "weapon_deagle" };
}

[MinimumApiVersion(130)]
public class NightVip : BasePlugin, IPluginConfig<NightVipConfig>
{
    public override string ModuleName => "NightVip";
    public override string ModuleVersion => "v1.5.1";
    public override string ModuleAuthor => "jockii";

    public static List<int?> _vips = new List<int?>();

    public static List<int> _noVipsRounds = new List<int>();

    private static Dictionary<gear_slot_t, uint> _constslot = new Dictionary<gear_slot_t, uint>();

    //public static Dictionary<CCSPlayerController, int> _jumps = new Dictionary<CCSPlayerController, int>(); // in v1.6.0 add Double Jump

    private static Dictionary<string, int> _weaponslot = new Dictionary<string, int>();

    public int _round = 0;

    public NightVipConfig Config { get; set; } = new NightVipConfig();

    public void OnConfigParsed(NightVipConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        if (Config.AutoGiveVip)
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);

        Console.WriteLine($"##########################################\n" +
            $"Plugin: {ModuleName} {ModuleVersion}\nAuthor: {ModuleAuthor}\nInfo: https://github.com/jockii\nSupport me: https://buymeacoffee.com/jockii\n" +
            $"##########################################");

        RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            var pl = @event.Userid;

            if (_vips.Contains(pl.UserId))
                _vips.Remove(pl.UserId);

            //if (_jumps.ContainsKey(pl))
            //    _jumps.Remove(pl);

            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundStart>((@event, info) =>
        {
            //clear _round
            var mp_maxrounds = ConVar.Find("mp_maxrounds");

            int serverMaxRounds = mp_maxrounds!.GetPrimitiveValue<int>();

            if (_round == serverMaxRounds)
                _round = 0;

            //for auto bhop
            if (Config.EnableAutoBhop)
            {
                var sv_cheats = ConVar.Find("sv_cheats");

                var sv_autobunnyhopping = ConVar.Find("sv_autobunnyhopping");

                bool cheatsValue = sv_cheats!.GetPrimitiveValue<bool>();

                bool bhopValue = sv_autobunnyhopping!.GetPrimitiveValue<bool>();

                TimeSpan.TryParse(Config.PluginStartTime, out TimeSpan start_time);
                TimeSpan.TryParse(Config.PluginEndTime, out TimeSpan end_time);
                TimeSpan.TryParse(DateTime.Now.ToString("HH:mm:ss"), out TimeSpan current_time);

                if (start_time <= current_time || current_time < end_time)
                {
                    if (bhopValue == false)
                    {
                        //enable
                        if (cheatsValue == false)
                            sv_cheats.SetValue(true);
                        sv_autobunnyhopping.SetValue(true);
                        sv_cheats.SetValue(false);
                    }
                    else
                        return HookResult.Continue;
                }
                else
                {
                    if (bhopValue == true)
                    {
                        //disable
                        if (cheatsValue == false)
                            sv_cheats.SetValue(true);
                        sv_autobunnyhopping.SetValue(false);
                        sv_cheats.SetValue(false);
                    }
                    else
                        return HookResult.Continue;
                }
            }

            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundEnd>((@event, info) =>
        {
            _round++;

            return HookResult.Continue;
        });

        // for no vip rounds
        AddTimer(1.0f, () =>
        {
            GetNoVipsRounds();
        });

        //create dictionary
        AddTimer(2.0f, () =>
        {
            CreateDictWeaponsNameSlot();
            CreateConstGearSlots();
        });
    }

    private void GetNoVipsRounds()
    {
        string[] _rounds = Config.DisableVipRounds.Split(',');

        foreach (var _round in _rounds)
        {
            int round = Convert.ToInt32(_round);
            _noVipsRounds.Add(round);
        }
    }

    private Dictionary<string, int> CreateDictWeaponsNameSlot()
    {
        _weaponslot = new Dictionary<string, int>()
        {
            { "weapon_glock",               1 },
            { "weapon_hkp2000",             1 },
            { "weapon_usp_silencer",        1 },
            { "weapon_p250",                1 },
            { "weapon_tec9",                1 },
            { "weapon_cz75a",               1 },
            { "weapon_fiveseven",           1 },
            { "weapon_deagle",              1 },
            { "weapon_revolver",            1 },
            { "weapon_mac10",               0 },
            { "weapon_mp9",                 0 },
            { "weapon_mp7",                 0 },
            { "weapon_mp5sd",               0 },
            { "weapon_ump45",               0 },
            { "weapon_p90",                 0 },
            { "weapon_bizon",               0 },
            { "weapon_nova",                0 },
            { "weapon_xm1014",              0 },
            { "weapon_sawedoff",            0 },
            { "weapon_mag7",                0 },
            { "weapon_m249",                0 },
            { "weapon_negev",               0 },
            { "weapon_galil",               0 },
            { "weapon_famas",               0 },
            { "weapon_ak47",                0 },
            { "weapon_m4a1",                0 },
            { "weapon_m4a1_silencer",       0 },
            { "weapon_ssg08",               0 },
            { "weapon_sg556",               0 },
            { "weapon_aug",                 0 },
            { "weapon_awp",                 0 },
            { "weapon_g3sg1",               0 },
            { "weapon_scar20",              0 },
            { "weapon_taser",               2 },
            { "weapon_molotov",             3 },
            { "weapon_incgrenade",          3 },
            { "weapon_decoy",               3 },
            { "weapon_flashbang",           3 },
            { "weapon_hegrenade",           3 },
            { "weapon_smokegrenade",        3 }
        };

        return _weaponslot;
    }

    private Dictionary<gear_slot_t, uint> CreateConstGearSlots()
    {
        _constslot = new Dictionary<gear_slot_t, uint>
        {
            { gear_slot_t.GEAR_SLOT_FIRST,              0},
            { gear_slot_t.GEAR_SLOT_PISTOL,             1},
            { gear_slot_t.GEAR_SLOT_KNIFE,              2},
            { gear_slot_t.GEAR_SLOT_GRENADES,           3},
            { gear_slot_t.GEAR_SLOT_C4,                 4},
            { gear_slot_t.GEAR_SLOT_BOOSTS,             11},
            { gear_slot_t.GEAR_SLOT_COUNT,              13},
            { gear_slot_t.GEAR_SLOT_UTILITY,            12},
            { gear_slot_t.GEAR_SLOT_INVALID,    4294967295},
            { gear_slot_t.GEAR_SLOT_RESERVED_SLOT6,      5},
            { gear_slot_t.GEAR_SLOT_RESERVED_SLOT7,      6},
            { gear_slot_t.GEAR_SLOT_RESERVED_SLOT8,      7},
            { gear_slot_t.GEAR_SLOT_RESERVED_SLOT9,      8},
            { gear_slot_t.GEAR_SLOT_RESERVED_SLOT10,     9},
            { gear_slot_t.GEAR_SLOT_RESERVED_SLOT11,    10},
        };

        return _constslot;
    }

    private void GiveWeaponItem(CCSPlayerController pl, string item)
    {
        if (item == null || item == "") return;

        int itemSlot = _weaponslot[item];

        var weapons = pl.PlayerPawn.Value?.WeaponServices;

        Dictionary<string, gear_slot_t> playerActiveSlots = new Dictionary<string, gear_slot_t>();

        foreach (var weapon in weapons!.MyWeapons)
        {
            if (weapon == null || !weapon.IsValid || !weapon.Value!.IsValid) continue;

            if (itemSlot > 3) continue;

            CCSWeaponBaseVData? _weapon = weapon?.Value?.As<CCSWeaponBase>().VData;

            if (_weapon == null) continue;

            playerActiveSlots.Add(_weapon.Name, _weapon.GearSlot);
        }

        if (!_weaponslot.ContainsKey(item))
            return;

        if (playerActiveSlots.ContainsKey(item))
            return;

        if (!playerActiveSlots.ContainsKey(item))
        {
            if (playerActiveSlots.ContainsValue((gear_slot_t)itemSlot))
            {
                if ((gear_slot_t)itemSlot == gear_slot_t.GEAR_SLOT_GRENADES)
                {
                    if (playerActiveSlots.ContainsKey(item))
                        return;
                    else
                        pl.GiveNamedItem(item);
                }
                else
                    return;
            }
            else
                pl.GiveNamedItem(item);
        }
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
        TimeSpan.TryParse(DateTime.Now.ToString("HH:mm:ss"), out TimeSpan current_time);

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
        TimeSpan.TryParse(DateTime.Now.ToString("HH:mm:ss"), out TimeSpan current_time);

        if (start_time <= current_time || current_time < end_time)
        {
            if (pl == null || !pl.IsValid || !pl.Pawn.IsValid || pl.IsBot || pl.IsHLTV) return HookResult.Continue;

            if (_vips.Contains(pl.UserId))
            {
                if (_noVipsRounds.Contains(_round)) return HookResult.Continue;

                AddTimer(1.0f, () =>
                {
                    if (Config.GivePlayerWeapons)
                    {
                        for (int i = 0; i < Config.WeaponsList.Count; i++)
                        {
                            if (i > Config.WeaponsList.Count)
                                break;

                            GiveWeaponItem(pl, Config.WeaponsList[i]!);
                        }
                    }

                    pl.Pawn.Value!.Health = Config.Health;
                    pl.PlayerPawn.Value!.ArmorValue = Config.Armor;
                    pl.PawnHasHelmet = true;
                    pl.InGameMoneyServices!.Account = Config.Money;
                    pl.Pawn.Value.GravityScale = Config.Gravity;

                    if (Config.GiveHealthShot)
                        pl.GiveNamedItem("weapon_healthshot");

                    if (Config.UseScoreBoardTag)
                        pl.Clan = Config.ScoreBoardTag;
                });
            }

            if (!_vips.Contains(pl.UserId)) return HookResult.Continue;
        }
        else
        {
            if (_vips.Count > 0)
                _vips.Clear();
            else
                return HookResult.Continue;
        }

        return HookResult.Continue;
    }
}