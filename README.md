# pcs-preservation-api
REST API for the preservation module in Project Completion System (ProCoSys (PCS)) using ASP.NET and Entity Framework.

## CI/CD Workflow
> [!IMPORTANT]  
> Changes merged with main will be built and deployed to test & production without any additional approval steps.

We continuously build, test, verify, and deploy changes to production. 

Changes that are ready for verification are entered into a pull request to the main branch and will be automatically deployed to the Test environment for verification. Once verified and the PR is merged, the changes will automatically be deployed to test and production environments.

For further details check out our development workflow [here](./CONTRIBUTING.md)

### Pull Requests
Pull requests to main branch are automatically deployed to Dev and require verification there.

### Test
Merging into main branch triggers an automatic deployment to Test.

### Production
Merging into main branch triggers an automatic deployment to Production.

### Manual deployment to environments
Dev, Test & Production deployment pipelines can be manually invoked.

To run manual GitHub deployment actions 
1. Select workflow on the left side
2. Click `Run workflow`
3. Select branch
4. Click `Run workflow`
