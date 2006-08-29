# Author: Matthew Metnetsky <matthew@cowarthill.com>
#
# Useful link -> http://dev.panopticsearch.com/make-notes.html
##############################################################

# if not linux
ifdef COMSPEC
	TR = '\\\\'
	CSC = /cygdrive/c/WINDOWS/Microsoft.NET/Framework/v2.0/csc.exe
else
	TR = '/'
	CSC = gmcs
	EXE = mono
endif

##############################################################################
# Variables
############

build = bin
deps = $(build)/deps

ass.bin = $(build)/$(ass)
ass.sources = $(deps)/$(ass).sources
ass.make = $(deps)/$(ass).make

# override for distribution compilation (no console logging in a release!!)
target = library
ifdef TARGET
	target = $(TARGET)
endif

ifdef KEYFILE
	keyfile = -keyfile:$(KEYFILE)
endif

##############################################################################
# Begin Rules
##############

$(ass.bin): $(build) $(ass.sources)
	$(CSC) $(keyfile) -d:NET_2_0=MONO /target:$(target) /out:$@ $(LIBRARIES) @$(ass.sources)

# Build ass.sources
ifneq ($(ass.sources),$(ass.stamp))
$(ass.sources): $(deps)
	@ls */*.cs | uniq -u | tr '/' '$(TR)' > $@
endif

# Build make frag which forces all *.cs files to be depencies of CMN.Util.dll
$(ass.make): $(ass.sources)
	@sed 's,^,$(ass.bin): ,' $< >$@

-include $(ass.make)

# create build directory and its children if its not there
$(build):
	@mkdir -p $@

$(deps):
	@if test ! -d $@; then mkdir $@; fi;

# record out latest build
$(ass.stamp):
	@touch $@

clean:
	rm -rf $(ass.bin) $(deps)

distclean: clean

.PHONY: run dist clean distclean

