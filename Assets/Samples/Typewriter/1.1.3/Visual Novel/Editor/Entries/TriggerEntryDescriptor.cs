using Aarthificial.Typewriter.Editor.Descriptors;

/// <summary>
/// A descriptor for <see cref="TriggerEntry"/>.
/// </summary>
/// <remarks>
/// Descriptors are used to control how custom entries are handled by the
/// Typewriter editor.
///
/// In this example we change the display name to "Trigger" and the color to
/// a bright red.
/// </remarks>
[CustomEntryDescriptor(typeof(TriggerEntry))]
public class TriggerEntryDescriptor : EventEntryDescriptor {
  public override string Name => "Trigger";
  public override string Color => "#0e8307";
}
