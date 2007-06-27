MFLAGS = -nologo -d:NET_2_0=MONO -keyfile:../PMS.snk

ASSEMBLY = pms-repo.exe

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:../PMS/PMS.dll \
	-pkg:printgroove-liblog4net2.0

SOURCES = \
	 ./Program.cs \
	 ./Properties/AssemblyInfo.cs

pms_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MFLAGS) -target:exe -out:$@ $(REFERENCES) $(SOURCES)
