# How to Release

This repository is configured to release new versions of the WaySON package via automated CI using GitHub Actions. Publishing of releases and previews follow slightly different workflows. See below for details.

## How to Publish a Release

In order to publish a new release, bump the package version in [WaySON.csproj](src/WaySON/WaySON.csproj) to a new version. On merge into the primary branch, this version change will kick off publishing a release for that version. Ensure that versioning changes adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html) and update our [CHANGELOG](CHANGELOG.md) file accordingly.

## How to Publish a Preview

In order to publish a preview, tag the commit with a preview tag that follows one of the tag formats defined in the [publish-preview](.github/workflows/publish-preview.yml) workflow. This will kick off publishing a preview package with a version that matches the preview tag.
