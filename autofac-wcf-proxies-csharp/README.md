# Castle.Core proxy (interceptor) + Autofac used for hiding WCF proxy

## Why

* WCF calls all over the place -> untestable code (explicit use of new WcfProxy<IContract>)
* smaller facades over existing oversized contract

## Why not

* well, it hides heavy network calls
* has some footprint
* stacktrace for exceptions is changed

## How

* interceptor which "generates" contract implementation on-the-fly
* on time proxy generation cost at registration (injected code to AppDomain - this code always stays there - be careful)
* small footprint (relativly to WCF) - 0.2ms per call