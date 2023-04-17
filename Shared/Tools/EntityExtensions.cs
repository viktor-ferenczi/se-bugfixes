using VRage.Game.Entity;

namespace Shared.Tools
{
    public static class EntityExtensions
    {
        public static string DebugNameNoId(this MyEntity entity) => entity.DisplayName ?? entity.Name ?? "UNNAMED";
        public static string DebugPosition(this MyEntity entity) => entity.PositionComp?.GetPosition().ToString() ?? "NO-POSITION";
    }
}