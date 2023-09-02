using Aarthificial.Typewriter.Editor.Descriptors;

/// <summary>
/// A descriptor for <see cref="DecisionEntry"/>.
/// </summary>
/// <remarks>
/// Descriptors are used to control how custom entries are handled by the
/// Typewriter editor.
///
/// In this example we change the display name to "Decision" and the color to
/// a bright red.
/// </remarks>
[CustomEntryDescriptor(typeof(DecisionEntry))]
public class DecisionEntryDescriptor : EventEntryDescriptor {
  public override string Name => "Decision";
  public override string Color => "#81f87a";
}
