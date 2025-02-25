using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.IntegrationTests;

/// <summary>
/// An attribite to ignore tests when running in an CI environment.
/// </summary>
/// <param name="mode">The condition mode.</param>
/// <remarks>code from https://github.com/dotnet/sdk/blob/main/src/Cli/dotnet/Telemetry/CIEnvironmentDetectorForTelemetry.cs.</remarks>
public sealed class CIConditionAttribute(ConditionMode mode) : ConditionBaseAttribute(mode)
{
    // Systems that provide boolean values only, so we can simply parse and check for true
    private static readonly string[] BooleanVariables = [

        // Azure Pipelines - https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables#system-variables-devops-services
        "TF_BUILD",

        // GitHub Actions - https://docs.github.com/en/actions/learn-github-actions/environment-variables#default-environment-variables
        "GITHUB_ACTIONS",

        // AppVeyor - https://www.appveyor.com/docs/environment-variables/
        "APPVEYOR",

        // A general-use flag - Many of the major players support this: AzDo, GitHub, GitLab, AppVeyor, Travis CI, CircleCI.
        // Given this, we could potentially remove all of these other options?
        "CI",

        // Travis CI - https://docs.travis-ci.com/user/environment-variables/#default-environment-variables
        "TRAVIS",

        // CircleCI - https://circleci.com/docs/2.0/env-vars/#built-in-environment-variables
        "CIRCLECI",
    ];

    // Systems where every variable must be present and not-null before returning true
    private static readonly string[][] AllNotNullVariables = [

        // AWS CodeBuild - https://docs.aws.amazon.com/codebuild/latest/userguide/build-env-ref-env-vars.html
        ["CODEBUILD_BUILD_ID", "AWS_REGION"],

        // Jenkins - https://github.com/jenkinsci/jenkins/blob/master/core/src/main/resources/jenkins/model/CoreEnvironmentContributor/buildEnv.groovy
        ["BUILD_ID", "BUILD_URL"],

        // Google Cloud Build - https://cloud.google.com/build/docs/configuring-builds/substitute-variable-values#using_default_substitutions
        ["BUILD_ID", "PROJECT_ID"]
    ];

    // Systems where the variable must be present and not-null
    private static readonly string[] IfNonNullVariables = [

        // TeamCity - https://www.jetbrains.com/help/teamcity/predefined-build-parameters.html#Predefined+Server+Build+Parameters
        "TEAMCITY_VERSION",

        // JetBrains Space - https://www.jetbrains.com/help/space/automation-environment-variables.html#general
        "JB_SPACE_API_URL"
    ];

    public override string? IgnoreMessage => "Skipped in CI runs";

    public override string GroupName => "CICondition";

    public override bool ShouldRun
    {
        get
        {
            foreach (var booleanVariable in BooleanVariables)
            {
                if (bool.TryParse(Environment.GetEnvironmentVariable(booleanVariable), out var envVar) && envVar)
                {
                    return true;
                }
            }

            foreach (var variables in AllNotNullVariables)
            {
                if (variables.All((variable) => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(variable))))
                {
                    return true;
                }
            }

            foreach (var variable in IfNonNullVariables)
            {
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(variable)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
