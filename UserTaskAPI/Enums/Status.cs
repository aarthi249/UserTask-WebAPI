using System.Runtime.Serialization;

namespace UserTaskAPI.Enums
{
    public enum Status
    {
        [EnumMember(Value = "NotStarted")]
        NotStarted,

        [EnumMember(Value = "Started")]
        Started,
        
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Completed")]
        Completed
    }
}
