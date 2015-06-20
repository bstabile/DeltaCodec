Stability.Data.Compression.ThirdParty (optional "DeltaCodec" extensions)

The Third-Party software included in this assembly has been extracted and simplified
from publicly available libraries that have open source licensing of various flavors.
This assembly is NOT intended for usage in closed-source production software. It is
only meant to provide some basic performance comparisons and benchmark information.

Most of these algorithms have native versions that would compare better in tests. 
But it was decided to use only pure C# versions in order to provide a level playing
field. I have no doubt that each technique could be further optimized to enhance
performance. However, no attempt has been made to alter the source any more than
necessary to fit in comforatably with the DeltaCodec framework.

In the case of QuickLZ, a commercially licensed version is available that has more
options and better performance characteristics. Please visit their website (quicklz.com)
if you are interested in integrating that with the framework.

Some of the other implementations may also have commercial versions available.
So please visit their sites (CodePlex, etc.) to find out the latest information.

In the special case of FPC, the C# version does not perform well enough to warrant 
deep investigation. It seems as though hardware optimizations available in native 
versions are very important to achieve the claimed performance advantages when 
compressing double-precision floating point series. Any applicable patents should 
obviously be respected. It has only been included here to make users aware of its 
availability.

The Ionic "DotNetZip" BZip2 implementation includes a parallel output stream type.
Although you might be tempted to use this instead of the serial version I would
strongly advise that you avoid it. It worked fine on the modest workstation used
for comparative testing. But I later tested it on a Thinkpad with twice the cores 
and tests failed because of synchronization problems. This must have something to 
do with the mix of an older threading model with the newer TPL. Just stay away from 
it and trust the "Parallel Block Encoding" provided (use the "numBlocks" parameter).  
In general, avoid getting too fancy with threading. There is only so much you can 
do in parallel on any given platform, and sticking to a consistent model is a good 
way to stay out of trouble.

All algorithms (except for FPC) are general purpose byte stream compressors.
That makes them suitable as "Finishers" that can be executed after various
"Transforms" are applied for delta or differencing of instrinsic data series.

Please read the information contained in the "Licensing" folder to determine
which implementations might be compatible with your own needs. When you have
analyzed the different performance characteristics of the various techniques,
you might want to create a separate customized library where you can focus
on optimization that best fits the kinds of data you are dealing with.

I would enthusiastically welcome and encourage the contribution of additional
libraries that are, at a minimum, compatible with GPL3 open source licensing.
It is my hope that a diverse set of time series compression techniques can be
made available to as many .NET developers as possible.

My company develops a highly optimized commercial codec (RWC) that we use for 
efficient storage and transmission of high-volume time series data. We use the 
exact same "scaffolding" that is made available with the DeltaCodec library.
We include RWC results in the reference performance reports in the "Docs" folder. 
This is done to set a higher bar for further optimization of open source codecs. 

Feel free to contact me via email with questions, suggestions, and comments.

Happy Encoding!

Bennett R. Stabile
Bennett.Stabile@gmail.com

Stability Systems, LLC
2015

