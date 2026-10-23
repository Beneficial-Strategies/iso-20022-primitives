# Changelog

The versions of this package have three parts, `design.breaking.compatible`: the first changes with a redesign, the second with a change that breaks existing users, the third with every other change. Newest first. Each entry is written from the issues closed since the previous release.

## 1.0.0-alpha (2026-10-23)

* Changed: The package lives in its own repository, with its history, and is versioned and released independently of the other packages. Its code is unchanged from 0.7.0-alpha.
* Changed: Versions have three parts, `design.breaking.compatible`: the first changes with a redesign, the second with a change that breaks existing users, the third with every other change. This is not Semantic Versioning.
* Changed: The README is the package README, and its examples are projects under `examples/` that are built and run against the packed package.
* Fixed: The first example of the README did not compile for lack of `using BeneficialStrategies.Iso20022;`.

## 0.7.0-alpha

Released from the monorepo this package lived in before (Iso20022Library), together with the other packages; its history is in this repository's commits and tags.
