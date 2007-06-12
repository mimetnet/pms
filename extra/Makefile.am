POLICY=policy.0.6.PMS.config
POLICY_DLL=policy.0.6.PMS.dll

CLEANFILES = $(POLICY) $(POLICY_DLL)
EXTRA_DIST = policy.config.in $(POLICY)

noinst_SCRIPTS = $(POLICY_DLL)

$(POLICY): policy.config
	cp $< $@

$(POLICY_DLL): $(POLICY)
	$(AL) -link:$(POLICY) -out:$(POLICY_DLL) -keyfile:../PMS.snk

install-data-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -i $(POLICY_DLL)

uninstall-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -u $(POLICY_DLL)
