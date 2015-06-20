Folder:	Stability.Data.Compression.Tests.Data 

This is where we will keep a few data samples that we want to test against for specific reasons.
Mainly, we want to avoid running all tests against data that is "generated" artificially.

****************************************************************************************************************
WHITE NOISE: H2-STRAIN_4096Hz-815045078-256.defb (1 million doubles, scale 16, precision 15)

	This file contains sample data used by LIGO detectors (Laser Interferometer Graviational Wave Observatory).
	It is basically a measurement of comsic background radiation that was captured during a 
	"hard-spectrum gamma-ray burst (GRB)" on 2005-11-03. You can read more about this at...
		http://www.ligo.org/scientists/GRB051103/index.php

	The relevant aspect for a compression library test is that cosmic background radiation is (as far as
	we are concerned) nearly pure Gaussian white noise. Scientists hoped that their LIGO detectors would be able to
	measure some effect of a GRB that might help support current gravitational wave theory. Alas, this
	was apparently not to be. 
	
	So we have here a sample of "natural" data collected by highly calibrated measurement devices and 
	normalized for expected perturbations by supercomputers. To say that there is very little "regularity"
	in the data might be a bit of an understatement. There is just enough non-randomness to allow SOME
	compression, without any "embarrassingly regular" moments.

	The file is stored in Deflate format so that it can be reconstituted using DeflateStream for the purpose
	of running the tests.

	NOTE: Most of the codecs cannot do better than a multiple of about 1.08X. With RWC we get about 1.4X.
****************************************************************************************************************
	