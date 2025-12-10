# Contributing

This repo uses [Trunk based development](https://trunkbaseddevelopment.com/) along with [Conventional commits](https://www.conventionalcommits.org/en/v1.0.0/). When making contributions follow this checklist:

- Ensure you have created an issue in Github Issues
- Create a new feature branch from the `main` branch
- Implement
- Perform a self-review of your code and ensure all CI is run before submitting the PR for approval.

## How we work

The following chart describes the development workflow for contributing changes to the repository.

```mermaid
flowchart TB

    deploy-dev>Dev deployment]
    deploy-test>Test deployment]
    deploy-prod>Prod deployment]
    main(main)
    merge(Squash and merge with main)
    pull-request(((Pull request)))
    
    subgraph Implementation
        implement(Implement change)
        validate-changes(Build, test and verify)
    end

    subgraph validation[Review and test]
        Approved
        improve(Make Improvements)
        rejected[Improvement requested]
        review{Code review}
        uat{User acceptance testing}
        validate-improvements(Build, test and verify)
    end

    main -- Create new branch --> implement

    implement --> validate-changes
    validate-changes --> | Create | pull-request
    
    pull-request -. Run via Github Actions .-> deploy-dev
    pull-request --> | If needed | uat
    pull-request --> review

    uat --> rejected
    review --> rejected
    rejected --> improve
    improve --> validate-improvements
    validate-improvements --> | Update | pull-request

    uat --> Approved
    review --> Approved
    Approved --> merge

    merge -. Run via Github Actions .-> deploy-test
    merge -. Run via Github Actions .-> deploy-prod
```
