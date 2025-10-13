using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._NC.Vehicles.Components
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class VehicleDamageComponent : Component
    {
        /// <summary>
        /// Максимальное здоровье двигателя.</summary>
        [DataField, AutoNetworkedField]
        public float MaxEngineHealth { get; set; } = 100f;

        /// <summary>
        /// Текущее здоровье двигателя.</summary>
        [DataField, AutoNetworkedField]
        public float CurrentEngineHealth { get; set; } = 100f;
    }
}
