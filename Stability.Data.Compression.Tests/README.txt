Stability.Data.Compression.Tests

"DeltaCodec" is a framework for building time series codecs that directly process lists
(vectors) of intrinsic data types, or optionally, more complex data structures. a pair
of simple codecs is supplied in the core library. Others are made available in the
"ThirdParty" assembly.

The usual suites of unit tests have not yet made if over from our commercial source tree.
Hopefully, at some point in the future we will add these to the open source version.
 
The "ComparativeTests" are set up in an unusual way. They are not really traditional
unit tests. Instead, they produce output that allows users to easily see relative
performance for various configurations across multiple codec types using different
sets of arguments.

The results don't format well with MSTest in Visual Studio. Everything was designed
to produce nice tabular results when run in NUnit (Resharper is fine if you have it).

There are a few lines of static configuration at the top of the test class. You can
play around with that to universally alter the list size, the run count, and some
alternative "groups" of preconfigured codec tests.

The test utility types that are used to coordinate all of this are somewhat complicated.
But unless you really must know how it all works you can largely ignore all of that.
To get started you can just run a few different configurations and observe the differences.

If you really want more advanced testing in, say, a WPF application, then you can
study the TestUtility assembly and use those classes directly to suit your needs.
Or you might want to run batteries of tests and store the information in a database, 
and then display it in web pages, or whatever.

Feel free to contact me via email if you have any questions, suggestions, or comments.

Happy Testing!

Bennett R. Stabile
Bennett.Stabile@gmail.com

Stability Systems, LLC
2015
