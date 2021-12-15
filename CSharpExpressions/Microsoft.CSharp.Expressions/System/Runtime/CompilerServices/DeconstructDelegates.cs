// Prototyping extended expression trees for C#.
//
// bartde - December 2021

namespace System.Runtime.CompilerServices
{
    public delegate void DeconstructAction<TObject, TOutput1>(TObject input, out TOutput1 output1);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2>(TObject input, out TOutput1 output1, out TOutput2 output2);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9, TOutput10>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9, out TOutput10 output10);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9, TOutput10, TOutput11>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9, out TOutput10 output10, out TOutput11 output11);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9, TOutput10, TOutput11, TOutput12>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9, out TOutput10 output10, out TOutput11 output11, out TOutput12 output12);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9, TOutput10, TOutput11, TOutput12, TOutput13>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9, out TOutput10 output10, out TOutput11 output11, out TOutput12 output12, out TOutput13 output13);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9, TOutput10, TOutput11, TOutput12, TOutput13, TOutput14>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9, out TOutput10 output10, out TOutput11 output11, out TOutput12 output12, out TOutput13 output13, out TOutput14 output14);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9, TOutput10, TOutput11, TOutput12, TOutput13, TOutput14, TOutput15>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9, out TOutput10 output10, out TOutput11 output11, out TOutput12 output12, out TOutput13 output13, out TOutput14 output14, out TOutput15 output15);
    public delegate void DeconstructAction<TObject, TOutput1, TOutput2, TOutput3, TOutput4, TOutput5, TOutput6, TOutput7, TOutput8, TOutput9, TOutput10, TOutput11, TOutput12, TOutput13, TOutput14, TOutput15, TOutput16>(TObject input, out TOutput1 output1, out TOutput2 output2, out TOutput3 output3, out TOutput4 output4, out TOutput5 output5, out TOutput6 output6, out TOutput7 output7, out TOutput8 output8, out TOutput9 output9, out TOutput10 output10, out TOutput11 output11, out TOutput12 output12, out TOutput13 output13, out TOutput14 output14, out TOutput15 output15, out TOutput16 output16);

    internal static class DeconstructActionDelegateHelpers
    {
        internal static Type GetDeconstructActionType(Type[] types) =>
            types.Length switch
            {
                2 => typeof(DeconstructAction<,>).MakeGenericType(types),
                3 => typeof(DeconstructAction<,,>).MakeGenericType(types),
                4 => typeof(DeconstructAction<,,,>).MakeGenericType(types),
                5 => typeof(DeconstructAction<,,,,>).MakeGenericType(types),
                6 => typeof(DeconstructAction<,,,,,>).MakeGenericType(types),
                7 => typeof(DeconstructAction<,,,,,,>).MakeGenericType(types),
                8 => typeof(DeconstructAction<,,,,,,,>).MakeGenericType(types),
                9 => typeof(DeconstructAction<,,,,,,,,>).MakeGenericType(types),
                10 => typeof(DeconstructAction<,,,,,,,,,>).MakeGenericType(types),
                11 => typeof(DeconstructAction<,,,,,,,,,,>).MakeGenericType(types),
                12 => typeof(DeconstructAction<,,,,,,,,,,,>).MakeGenericType(types),
                13 => typeof(DeconstructAction<,,,,,,,,,,,,>).MakeGenericType(types),
                14 => typeof(DeconstructAction<,,,,,,,,,,,,,>).MakeGenericType(types),
                15 => typeof(DeconstructAction<,,,,,,,,,,,,,,>).MakeGenericType(types),
                16 => typeof(DeconstructAction<,,,,,,,,,,,,,,,>).MakeGenericType(types),
                17 => typeof(DeconstructAction<,,,,,,,,,,,,,,,,>).MakeGenericType(types),
                _ => null
            };
    }
}
