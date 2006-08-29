SUBDIRS = PMS PMS.NUnit.Model PMS.NUnit

all:
	for i in $(SUBDIRS); do	$(MAKE) -C $$i || exit; done

clean:
	for i in $(SUBDIRS); do	$(MAKE) -C $$i clean || exit; done

distclean:
	for i in $(SUBDIRS); do	$(MAKE) -C $$i distclean || exit; done

.PHONY: all clean distclean

