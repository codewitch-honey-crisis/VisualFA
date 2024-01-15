namespace System
{
	// dummy for DNF so Slang can compile
	internal class ReadOnlySpan<T>
	{
		public override string ToString()
		{
			return null;
		}
		public int Length { get { return 0; } }
		public ReadOnlySpan<T> Slice(int position, int length)
		{
			return null;
		}
	}
}
