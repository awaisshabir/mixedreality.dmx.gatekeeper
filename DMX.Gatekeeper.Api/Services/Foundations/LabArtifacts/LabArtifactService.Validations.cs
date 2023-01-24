// ---------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

using System.IO;
using DMX.Gatekeeper.Api.Models.LabArtifacts;
using DMX.Gatekeeper.Api.Models.LabArtifacts.Exceptions;

namespace DMX.Gatekeeper.Api.Services.Foundations.LabArtifacts
{
    public partial class LabArtifactService : ILabArtifactService
    {
        private static void ValidateLabArtifactPropertiesOnAdd(string labArtifactName, Stream labArtifactContent)
        {
            Validate(
                (Rule: IsInvalid(labArtifactName), Parameter: nameof(LabArtifact.Name)),
                (Rule: IsInvalid(labArtifactContent), Parameter: nameof(LabArtifact.Content)));
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required",
        };

        private static dynamic IsInvalid(Stream content) => new
        {
            Condition = content == null,
            Message = "Content is required",
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidLabArtifactException = new InvalidLabArtifactException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidLabArtifactException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidLabArtifactException.ThrowIfContainsErrors();
        }
    }
}