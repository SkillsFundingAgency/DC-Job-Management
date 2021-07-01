using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Tests.Builders;

namespace ESFA.DC.PeriodEnd.Tests
{
    public class PeriodEndProviderReportReadyData
    {
        public static IEnumerable<object[]> Data()
        {
            return new List<object[]>
            {
                SingleProviderReportPathItemWithSingleCompletedJobShouldBeReady(),
                SingleProviderReportPathItemWithCompletedAndFailedJobsShouldBeReady(),
                SingleProviderReportPathItemWithMultipleCompletedJobsShouldBeReady(),
                SingleProviderReportPathItemWithSingleIncompleteJobShouldNotBeReady(),
                SingleProviderReportPathItemWithMultipleIncompleteJobsShouldNotBeReady(),
                SingleProviderReportPathItemWithMultipleJobsAndOnlyOneCompletedShouldNotBeReady(),
                SingleProviderReportPathItemWithNoPathItemJobsShouldNotBeReady(),
                SingleProviderReportPathItemWithEmptyPathItemJobsCollectionShouldNotBeReady(),
                MultipleProviderReportPathItemWithOnlyOneWithCompletedJobsShouldNotBeReady(),
                MultiplePathPathItemsEachWithProviderReportButOnlyOneWithCompletedShouldNotBeReady(),
                MultiplePathPathItemsAllWithCompletedProviderReportsShouldBeReady(),
                MultiplePathPathItemsButOnlyOneWithProviderReportsShouldBeReady(),
                SingleProviderReportPathItemWithMultiplePathItemsJobsAndSingleCompletedReportJobShouldBeReady(),
                MultipleProviderReportPathItemWithOneCompletedJobAndOneEmptyJobShouldNotBeReady(),
                MultiplePathPathItemsAllWithCompletedOrFailedProviderReportsShouldBeReady()
            };
        }

        private static object[] SingleProviderReportPathItemWithSingleCompletedJobShouldBeReady()
        {
            return new object[]
            {
                true,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithCompletedAndFailedJobsShouldBeReady()
        {
            return new object[]
            {
                true,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithFailedStatus()
                                        .Build())
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithFailedRetryStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithMultiplePathItemsJobsAndSingleCompletedReportJobShouldBeReady()
        {
            return new object[]
            {
                true,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                         .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .Build())
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithMultipleCompletedJobsShouldBeReady()
        {
            return new object[]
            {
                true,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] MultipleProviderReportPathItemWithOnlyOneWithCompletedJobsShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                          .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithReadyStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] MultipleProviderReportPathItemWithOneCompletedJobAndOneEmptyJobShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                          .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithEmptyPathItemJobModelCollection()
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithSingleIncompleteJobShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithReadyStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithMultipleIncompleteJobsShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithReadyStatus()
                                        .Build())
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithReadyStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithMultipleJobsAndOnlyOneCompletedShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithReadyStatus()
                                        .Build())
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithNoPathItemJobsShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                           new PathItemModelBuilder()
                            .WithIsProviderReport()
                            .Build())
                        .Build()
                }
            };
        }

        private static object[] SingleProviderReportPathItemWithEmptyPathItemJobsCollectionShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                           new PathItemModelBuilder()
                            .WithEmptyPathItemJobModelCollection()
                            .WithIsProviderReport()
                            .Build())
                        .Build()
                }
            };
        }

        private static object[] MultiplePathPathItemsAllWithCompletedProviderReportsShouldBeReady()
        {
            return new object[]
            {
                true,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build(),
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] MultiplePathPathItemsAllWithCompletedOrFailedProviderReportsShouldBeReady()
        {
            return new object[]
            {
                true,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build(),
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithFailedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build(),
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithFailedRetryStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] MultiplePathPathItemsEachWithProviderReportButOnlyOneWithCompletedShouldNotBeReady()
        {
            return new object[]
            {
                false,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build(),
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithReadyStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build()
                }
            };
        }

        private static object[] MultiplePathPathItemsButOnlyOneWithProviderReportsShouldBeReady()
        {
            return new object[]
            {
                true,
                new List<PathPathItemsModel>
                {
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithCompletedStatus()
                                        .Build())
                                .WithIsProviderReport()
                                .Build())
                        .Build(),
                    new PathPathItemsModelBuilder()
                        .WithPathItemModel(
                            new PathItemModelBuilder()
                                .WithPathItemJobModel(
                                    new PathItemJobModelBuilder()
                                        .WithReadyStatus()
                                        .Build())
                                .Build())
                        .Build()
                }
            };
        }
    }
}
