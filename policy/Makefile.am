AUTOMAKE_OPTIONS = gnu

COMPAT_VERSIONS=0.6 0.7

POLICY_DLLS=$(patsubst %,policy.%.PMS.dll,$(COMPAT_VERSIONS))

CLEANFILES = $(POLICY_DLLS)
EXTRA_DIST = Makefile.in Makefile.am

noinst_SCRIPTS = $(POLICY_DLLS)

$(POLICY_DLLS): %.dll: %.config $(top_srcdir)/PMS/PMS.dll $(top_srcdir)/certs/PMS.snk
	$(AL) -link:$< -out:$@ -keyfile:$(top_srcdir)/certs/PMS.snk

install-data-local: $(POLICY_DLLS)
	for i in $^ ; do $(GACUTIL) $(GACUTIL_FLAGS) -i $$i ; done

uninstall-local:
	for i in $(POLICY_DLLS:%.dll=%) ; do $(GACUTIL) $(GACUTIL_FLAGS) -u $$i ; done
