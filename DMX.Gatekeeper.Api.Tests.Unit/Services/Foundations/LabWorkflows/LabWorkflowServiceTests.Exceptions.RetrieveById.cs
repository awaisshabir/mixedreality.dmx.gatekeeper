﻿// ---------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

using DMX.Gatekeeper.Api.Models.LabWorkflows;
using DMX.Gatekeeper.Api.Models.LabWorkflows.Exeptions;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xeptions;
using Xunit;

namespace DMX.Gatekeeper.Api.Tests.Unit.Services.Foundations.LabWorkflows
{
    public partial class LabWorkflowServiceTests
    {
        [Theory]
        [MemberData(nameof(CriticalDependencyExceptions))]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfCriticalErrorOccursAndLogItAsync(
            Xeption criticalDependencyException)
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            Guid someLabWorkflowId = randomGuid;

            var failedLabWorkflowDependencyException =
                new FailedLabWorkflowDependencyException(criticalDependencyException);

            var expectedLabWorkflowDependencyException =
                new LabWorkflowDependencyException(failedLabWorkflowDependencyException);

            this.dmxApiBrokerMock.Setup(broker =>
                broker.GetLabWorkflowByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(criticalDependencyException);

            // when
            ValueTask<LabWorkflow> retrieveLabWorkflowTask =
                this.labWorkflowService.RetrieveLabWorkflowByIdAsync(someLabWorkflowId);

            LabWorkflowDependencyException actualLabWorkflowDependencyException =
                await Assert.ThrowsAsync<LabWorkflowDependencyException>(
                    retrieveLabWorkflowTask.AsTask);

            // then
            actualLabWorkflowDependencyException.Should()
                .BeEquivalentTo(expectedLabWorkflowDependencyException);

            this.dmxApiBrokerMock.Verify(broker =>
                broker.GetLabWorkflowByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedLabWorkflowDependencyException))),
                        Times.Once());

            this.dmxApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyException))]
        public async Task ShouldThrowDependencyExceptionWhenDependencyErrorOccursOnRetrieveByIdAndLogItAsync(
            Exception dependencyException)
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid someLabWorkflowId = randomId;

            var failedLabWorkflowDependencyException =
                new FailedLabWorkflowDependencyException(dependencyException);

            var expectedLabWorkflowDependencyException =
                new LabWorkflowDependencyException(failedLabWorkflowDependencyException);

            this.dmxApiBrokerMock.Setup( broker =>
                broker.GetLabWorkflowByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<LabWorkflow> retrieveLbWorkflowByIdTask =
                this.labWorkflowService.RetrieveLabWorkflowByIdAsync(someLabWorkflowId);

            LabWorkflowDependencyException actualLabWorkflowDependencyException =
                await Assert.ThrowsAsync<LabWorkflowDependencyException>(
                    retrieveLbWorkflowByIdTask.AsTask);

            // then
            actualLabWorkflowDependencyException.Should().BeEquivalentTo(
                expectedLabWorkflowDependencyException);

            this.dmxApiBrokerMock.Verify(broker =>
                broker.GetLabWorkflowByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedLabWorkflowDependencyException))),
                        Times.Once);

            this.dmxApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveIfErrorOccursAndLogItAsync()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            Guid someGuid = randomGuid;

            var serviceException = new Exception();

            var failedLabWorkflowServiceException =
                new FailedLabWorkflowServiceException(serviceException);

            var expectedLabWorkflowServiceException =
                new LabWorkflowServiceException(failedLabWorkflowServiceException);

            this.dmxApiBrokerMock.Setup(broker =>
                broker.GetLabWorkflowByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<LabWorkflow> retrieveLabWorkflowByIdTask =
                this.labWorkflowService.RetrieveLabWorkflowByIdAsync(someGuid);

            LabWorkflowServiceException actualLabWorkflowServiceException =
                await Assert.ThrowsAsync<LabWorkflowServiceException>(
                    retrieveLabWorkflowByIdTask.AsTask);

            // then
            actualLabWorkflowServiceException.Should().BeEquivalentTo(
                expectedLabWorkflowServiceException);

            this.dmxApiBrokerMock.Verify(broker =>
                broker.GetLabWorkflowByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedLabWorkflowServiceException))),
                        Times.Once);

            this.dmxApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
