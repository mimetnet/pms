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

# override for distribution compilation (no console logging in a release!!)
target = library
ifdef TARGET
	target = $(TARGET)
endif

ifdef RELEASE
	mode = Release
	debug = /debug-
else
	mode = Debug
	debug = /debug+
endif

ifdef KEYFILE
	keyfile = -keyfile:$(KEYFILE)
endif

build = bin/$(mode)
deps = $(build)/deps

ass.bin = $(build)/$(ASSEMBLY)
ass.sources = $(deps)/$(ASSEMBLY).sources
ass.make = $(deps)/$(ASSEMBLY).make


##############################################################################
# Begin Rules
##############

$(ass.bin): $(build) $(ass.sources)
	$(CSC) $(debug) $(keyfile) -d:NET_2_0=MONO /target:$(target) /out:$@ $(LIBRARIES) @$(ass.sources)

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
	@mkdir -p $@

# record out latest build
$(ass.stamp):
	@touch $@

clean:
	rm -rf $(ass.bin) $(deps)

distclean: clean

all: $(ass.bin)

default: all

release:
	make default RELEASE=true

.PHONY: run dist clean distclean

