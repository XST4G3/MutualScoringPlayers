using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace MutualScoringPlayers;

class MutualScoring
{
    public int[] Kills { get; set; } = new int[65];
    public int[] KillsRow { get; set; } = new int[65];

    public void Clear()
    {
        Array.Clear(Kills);
        Array.Clear(KillsRow);
    }
}

public sealed class MutualScoringPlayers : BasePlugin
{
    public override string ModuleName => "MutualScoringPlayers";
    public override string ModuleAuthor => "AlmazON & xstage";
    public override string ModuleVersion => "0.0.1";

    private MutualScoring[] _mutualScoring = new MutualScoring[65];

    public override void Load(bool hotReload)
    {
        for (int i = 0; i < 65; ++i)
            _mutualScoring[i] = new MutualScoring();

        RegisterListener<Listeners.OnClientDisconnectPost>(slot =>
            _mutualScoring[slot + 1].Clear()
        );
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;
        var attacker = @event.Attacker;

        if (player == null || attacker == null || player == attacker)
            return HookResult.Continue;

        _mutualScoring[player.Index].KillsRow[attacker.Index] = 0;

        _mutualScoring[attacker.Index].Kills[player.Index]++;
        _mutualScoring[attacker.Index].KillsRow[player.Index]++;

        attacker.PrintToChat(
            string.Format(" \x04[MutualScoring] \x03{0} {1:d}\x01:\x05{2:d} {3} \x01(\x03+{4:d} \x01подряд)",
            attacker.PlayerName, _mutualScoring[attacker.Index].Kills[player.Index], _mutualScoring[player.Index].Kills[attacker.Index], player.PlayerName, _mutualScoring[attacker.Index].KillsRow[player.Index])
        );
        player.PrintToChat(
            string.Format(" \x04[MutualScoring] \x03{0} {1:d}\x01:\x05{2:d} {3} \x01(\x03-{4:d} \x01подряд)",
            player.PlayerName, _mutualScoring[player.Index].Kills[attacker.Index], _mutualScoring[attacker.Index].Kills[player.Index], attacker.PlayerName, _mutualScoring[attacker.Index].KillsRow[player.Index])
        );

        return HookResult.Continue;
    }
}