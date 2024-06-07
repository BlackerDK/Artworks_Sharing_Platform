
$(document).ready(function () {
    $('.setRoleBtn').click(function () {
        var itemId = $(this).data("id")
        var userId = $(this).data("userid")
        var selectedRoles = [];
        $("#userRole-" + itemId + " option:selected").each(function () {
            selectedRoles.push($(this).text());
        });
        console.log(selectedRoles);


        var data = {
            userId: userId,
            roleName: selectedRoles,
        };
        console.log("data" + data);
        $.ajax({
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            url: "/admin/AddUserRole",
            data: JSON.stringify(data),
            success: function (data) {
                window.location.reload();
            },
        });

    })
    $('.changeUserStatus').click(function () {
        var status = $(this).data("status")
        var userId = $(this).data("userid")

        var data = {
            userId: userId,
        };
        $.ajax({
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            url: "/admin/ChangeStatusUser",
            data: JSON.stringify(data),
            success: function (data) {
                window.location.reload();
            },
        });
    })

    window.loadPage = function (pages) {
        var data = {
            perPage: 10,
            currentPage: pages - 1,
            //sortBy: null,
            //isAscending : true,
        }
        $.ajax({
            type: "GET",
            url: "/admin/getUserRoles",
            data: data,
            success: function (data) {
                var updateContent = $(data).find('#tableContent').html();
                $(document).ready(function () {
                    $('#tableContent').html(updateContent);

                    let page = parseInt($('#selectedPage').val()) + 1;
                    let _pages = parseInt($('#totalPage').val());

                    $('.setRoleBtn').click(function () {
                        var itemId = $(this).data("id")
                        var userId = $(this).data("userid")
                        var selectedRoles = [];
                        $("#userRole-" + itemId + " option:selected").each(function () {
                            selectedRoles.push($(this).text());
                        });
                        console.log(selectedRoles);
                        var data = {
                            userId: userId,
                            roleName: selectedRoles,
                        };
                        $.ajax({
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            type: "POST",
                            url: "/admin/AddUserRole",
                            data: JSON.stringify(data),
                            success: function (data) {
                                $('.modal-backdrop').hide();
                                $('#userRoleModal-' + itemId).hide();
                                loadPage(page);
                            },
                        });
                    })
                    $('.changeUserStatus').click(function () {
                        var itemId = $(this).data("id")
                        var userId = $(this).data("userid")

                        var data = {
                            userId: userId,
                        };
                        $.ajax({
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            type: "POST",
                            url: "/admin/ChangeStatusUser",
                            data: JSON.stringify(data),
                            success: function (data) {
                                $('.modal-backdrop').hide();
                                $('#userRoleModal-' + itemId).hide();
                                loadPage(page);
                                loadPage(page);
                            },
                        });
                    })

                    createPagination(_pages, page)
                })
            },
        });
    }
    let page = parseInt($('#selectedPage').val()) + 1;
    let pages = parseInt($('#totalPage').val());
    window.createPagination = function (pages, page) {
        console.log("page ", pages);
        console.log("selected ", page);
        let str = '<ul>';
        let active;
        let pageCutLow = page - 1;
        let pageCutHigh = page + 1;
        // Show the Previous button only if you are on a page other than the first
        if (page > 1) {
            str += '<li class="page-item previous no"><a href="#" onclick="createPagination(' + pages + ', ' + (page - 1) + '), loadPage(' + (page - 1) + ')">Previous</a></li>';
        }
        // Show all the pagination elements if there are less than 6 pages total
        if (pages < 6) {
            for (let p = 1; p <= pages; p++) {
                active = page == p ? "active" : "no";
                str += '<li class="' + active + '"><a href="#" onclick="createPagination(' + pages + ', ' + p + '), loadPage(' + p + ')">' + p + '</a></li>';
            }
        }
        // Use "..." to collapse pages outside of a certain range
        else {
            // Show the very first page followed by a "..." at the beginning of the
            // pagination section (after the Previous button)
            if (page > 2) {
                str += '<li class="no page-item"><a href="#" onclick="createPagination(' + pages + ', 1), , loadPage(' + 1 + ')">1</a></li>';
                if (page > 3) {
                    str += '<li class="out-of-range"><a href="#" onclick="createPagination(' + pages + ',' + (page - 2) + '), loadPage(' + (page - 2) + ')">...</a></li>';
                }
            }
            // Determine how many pages to show after the current page index
            if (page === 1) {
                pageCutHigh += 2;
            } else if (page === 2) {
                pageCutHigh += 1;
            }
            // Determine how many pages to show before the current page index
            if (page === pages) {
                pageCutLow -= 2;
            } else if (page === pages - 1) {
                pageCutLow -= 1;
            }
            // Output the indexes for pages that fall inside the range of pageCutLow
            // and pageCutHigh
            for (let p = pageCutLow; p <= pageCutHigh; p++) {
                if (p === 0) {
                    p += 1;
                }
                if (p > pages) {
                    continue
                }
                active = page == p ? "active" : "no";
                str += '<li class="page-item ' + active + '"><a href="#" onclick="createPagination(' + pages + ', ' + p + ')", loadPage(' + (p) + ') >' + p + '</a></li>';
            }
            // Show the very last page preceded by a "..." at the end of the pagination
            // section (before the Next button)
            if (page < pages - 1) {
                if (page < pages - 2) {
                    str += '<li class="out-of-range"><a href="#" onclick="createPagination(' + pages + ',' + (page + 2) + '), loadPage(' + (page + 2) + ')">...</a></li>';
                }
                str += '<li class="page-item no"><a href="#" onclick="createPagination(' + pages + ', ' + pages + ')", loadPage(' + (pages) + ')>' + pages + '</a></li>';
            }
        }
        // Show the Next button only if you are on a page other than the last
        if (page < pages) {
            str += '<li class="page-item next no"><a href="#" onclick="createPagination(' + pages + ', ' + (page + 1) + '), loadPage(' + (page + 1) + ')">Next</a></li>';
        }
        str += '</ul>';
        // Return the pagination string to be outputted in the pug templates
        document.getElementById('pagination').innerHTML = str;
        return str;
    }

    document.getElementById('pagination').innerHTML = createPagination(pages, page);

});





