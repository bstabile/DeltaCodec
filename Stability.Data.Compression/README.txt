Stability.Data.Compression ("DeltaCodec")

This library provides some basic "scaffolding" to ease the implementation
of time series compression on the .NET platform.

There are several core types that help reduce the boilerplate code needed
to directly encode and decode lists of each instrinsic data type:

	IDeltaCodec
	IDeltaTransform
	IFinisher

An implementation of a codec includes an IDeltaTransform and IFinisher.

There are two predefined codecs in the core library and you can create 
your own more advanced implementations.

DeflateCodec has a NullTransform (which does nothing) and a DeflateFinisher 
which uses System.IO.Compression.DeflateStream to provide simple compression.

DeflateDeltaCodec has a DeltaTransform (which differences the series) and
a DeflateFinisher to provide final compression after transformation.

These simple codecs currently only handle individual vectors. It is
not difficult to implement codecs that can handle more complex data
structures. I can't know what kind of strongly typed structures developers 
might want to compress. But I might add an example codec that accepts lists
of Tuples. From there it should be trival to derive your own strongly-typed 
specializations. It will only involve specification of higher-order parameter
sets and framing semantics when combining multiple low-level vector encodings.

Codecs accept a set of parameters for vector encodings that help control 
the basic transformation and finishing behavior.

Transforms accept a single parameter, DeltaBlockState<T> that is used during
the encoding and decoding of a series. It helps to simplify setting up some 
of the required header information for each block. The codec itself manages
the final "frame header" when serializing one or more encoded blocks.

Finishers accept a stream or an IList<T> where T is an intrinsic data type.

Parallelism can be achieved by providing a "numBlocks" argument when encoding.
The speed can usually be greatly increased by using a value that is twice
the number of processors (Environment.ProcessorCount * 2). The default value
is 1, which can be useful when lots of ambient concurrency already exists.
Both the Transform and the Finisher are executed in the context of a block.

Example Usage: 

	var bytes = codec.Encode(
					IList<T> list, 
					CompressionLevel.Optimal, 
					T granularity, 
					Monotonicity.None, 
					numBlocks);

	var list = codec.Decode<T>(bytes);

Granularity can be useful when your data has a known discretization. It is
mainly useful in transforms but has little relevance for finishing compression.
If you think about it, this makes sense because many general purpose algorithms 
use frequency of symbols in one way or another to map to smaller representations.
But the purpose of transforms is different. They are meant to reduce the amount
of data that is sent to a finishing compressor, which can increase overall speed.
The data will be divided by this value, usually helping to increase the ratio.
If you use the default value of 0, the Transform will attempt to find the
granularity for you, but it will cost you in terms of time. If you specify 
a value of 1, then no factoring will be performed. If you specify a value 
that is not 0 or 1, then that granularity is used. Thus, if you know the
granularity ahead of time, it costs you little to provide it. But if your
transform can't do anything useful with the factored data, then you might
want to use 1 to avoid factoring completely.

Care must be exercised when using a granularity with Single and Double
precision data. To ensure completely lossless compression it is necessary
(or at least highly recommended) to specify a granularity of 1. That means
no division or multiplication will be performed, thus avoiding the problems
associated with these types. If you still want the data "granularized",
say because only a specific level of precision is of interest, then you
can often achieve better ratio. But with that the only guarantee is that 
input and output will be very close to equal. There can be small differences
at the bit level. In other words, if the precision of your granularity is 2, 
then when you compare ToString("F2") input and output should be equal.  

This precaution, of course, doesn't apply to Decimal, Integers,
(including Byte and SByte), or to DateTime/TimeSpan types. Those are
always completely lossless as long as the granularity reflects the
actual discretization of your data. You could always force a granularity
of say, 5, when the actual data has a finer grain. Again, you will often
increase the compression ratio but you will be losing precision.

Monotonicity might be useful for, say, DataTime or TimeSpan vectors if we 
know in advance that the data will be, for example, non-decreasing. However, 
in the basic transform included with the library, no such optimization has
been implemented. It is included for future experimentation.

All codec arguments are optional (except for the list itself, of course).
This makes it VERY easy to get started using the codecs without knowing
much about how any of it works. 

Use the provided unit tests to explore how various settings affect
performance with different types of time series data. A very basic
TimeSeriesProvider has been implemented to generate series for testing.
In the future, I will probably integrate "MathNet.Numerics" to the test
utility assembly to provide much more advanced series generation capabilities.
This should make it easier to create a full range of "edge" and "degenerate" 
cases.

My company develops a highly optimized commercial codec (RWC) that we use for 
efficient storage and transmission of high-volume time series data. We use the 
exact same "scaffolding" that is made available with the DeltaCodec library.
We include RWC results in the reference performance reports in the "Docs" folder. 
This is done to set a higher bar for further optimization of open source codecs. 

Feel free to contact me via email if you have any questions, suggestions, 
or comments.

Happy Encoding!

Bennett R. Stabile
Bennett.Stabile@gmail.com

Stability Systems, LLC
2015