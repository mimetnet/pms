MFLAGS = -debug+ -nologo -d:MONO -keyfile:../PMS.snk

ASSEMBLY = pms-providers.exe

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:../PMS/PMS.dll \
	-r:Mono.Posix.dll \
	-r:Mono.GetOptions.dll \
	-pkg:$(PKGLOG4NET)

SOURCES = \
	 ./Program.cs \
	 ./Properties/AssemblyInfo.cs

pms_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

EXTRA_DIST = $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MFLAGS) -target:exe -out:$@ $(REFERENCES) $(SOURCES)

run: $(ASSEMBLY)
	MONO_PATH=$(top_srcdir)/PMS/ $(MONO) --debug $<
